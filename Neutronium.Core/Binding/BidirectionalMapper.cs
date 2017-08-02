using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;
using System.Linq;
using MoreCollection.Extensions;

namespace Neutronium.Core.Binding
{
    public class BidirectionalMapper : IDisposable, IUpdatableJSCSGlueCollection, IJavascriptToCSharpConverter, IJavascriptChangesObserver
    {
        private HTMLViewContext _Context;
        private readonly IWebSessionLogger _Logger;
        private readonly JavascriptBindingMode _BindingMode;
        private readonly CSharpToJavascriptConverter _JSObjectBuilder;
        private IJavascriptObjectBuilderStrategy _JavascriptObjectBuilder;
        private IJavascriptSessionInjector _sessionInjector;
        private readonly SessionCacher _SessionCache;
        private IJSCSGlue _Root;
        private readonly FullListenerRegister _ListenerRegister;
        private readonly List<IJSCSGlue> _UnrootedEntities= new List<IJSCSGlue>();
        private bool _IsListening = false;
        private bool _IsLoaded = false;

        public IJSCSGlue JSValueRoot => _Root;
        public bool ListenToCSharp => (_BindingMode != JavascriptBindingMode.OneTime);
        public JavascriptBindingMode Mode => _BindingMode;
        public HTMLViewContext Context => _Context;
        private readonly object _RootObject;

        internal BidirectionalMapper(object root, HTMLViewEngine contextBuilder, JavascriptBindingMode mode, IWebSessionLogger logger):
            this(root, contextBuilder, null, mode, logger)
        {        
        }

        internal BidirectionalMapper(object root, HTMLViewEngine contextBuilder, IGlueFactory glueFactory, JavascriptBindingMode mode, IWebSessionLogger logger)
        {
            _BindingMode = mode;
            _Logger = logger;
            var javascriptObjecChanges = (mode == JavascriptBindingMode.TwoWay) ? (IJavascriptChangesObserver)this : null;
            _Context = contextBuilder.GetMainContext(javascriptObjecChanges);
            _SessionCache = new SessionCacher();
            _ListenerRegister = new FullListenerRegister(
                                        (n) => n.PropertyChanged += CSharpPropertyChanged,
                                        (n) => n.PropertyChanged -= CSharpPropertyChanged,
                                        (n) => n.CollectionChanged += CSharpCollectionChanged,
                                        (n) => n.CollectionChanged -= CSharpCollectionChanged,
                                        (c) => c.ListenChanges(),
                                        (c) => c.UnListenChanges());
            glueFactory = glueFactory ?? GlueFactoryFactory.GetFactory(_Context, this);
            _JSObjectBuilder = new CSharpToJavascriptConverter(contextBuilder.HTMLWindow, _SessionCache, glueFactory, _Logger);
            _RootObject = root;
        }

        internal async Task IntrospectVm(object addicionalObject) 
        {
            await _Context.RunOnUIContextAsync(() => 
            {
                _Root = _JSObjectBuilder.Map(_RootObject, addicionalObject);

                if (ListenToCSharp)
                    ListenToCSharpChanges();

                _IsListening = true;
            });
        }

        internal async Task UpdateJavascriptObjects(bool debugMode) 
        {
            await RunInJavascriptContext(async () => 
            {
                RegisterJavascriptHelper();

                _Context.InitOnJsContext(debugMode);
                _sessionInjector = _Context.JavascriptSessionInjector;

                _JavascriptObjectBuilder = _Context.WebView.GetBuildingStrategy(_SessionCache, _Context.JavascriptFrameworkIsMappingObject == false);
                _JavascriptObjectBuilder.UpdateJavascriptValue(_Root);

                var res = await InjectInHTMLSession(_Root);
                await _sessionInjector.RegisterMainViewModel(res);

                _IsLoaded = true;
            });
        }

        private void RegisterJavascriptHelper()
        {
            IJavascriptObject res;
            var resource = new ResourceReader("scripts", this);
            _Context.WebView.Eval(resource.Load("Infra.js"), out res);
        }

        private Task RunInJavascriptContext(Action run)
        {
            return _Context.WebView.RunAsync(run);
        }

        private Task RunInJavascriptContext(Func<Task> run)
        {
            return _Context.WebView.Evaluate(run);
        }

        private async Task<T> RunInJavascriptContext<T>(Func<Task<T>> run)
        {
            var res = await _Context.WebView.EvaluateAsync(run);
            return await res;
        }

        private Task<T> EvaluateInUIContextAsync<T>(Func<T> run)
        {
            return _Context.EvaluateOnUIContextAsync(run);
        }

        public void Dispose()
        {
            if (ListenToCSharp)
                UnlistenToCSharpChanges();

            _Context.Dispose();
            _UnrootedEntities.Clear();
            _Logger.Debug("BidirectionalMapper disposed");
        }

        private ISet<IJSCSGlue> GetAllChildren()
        {
            var @glues = _Root.GetAllChildren(true);
            @glues.UnionWith(_UnrootedEntities.SelectMany(ent => ent.GetAllChildren(true)));
            return @glues;
        }

        private void OnEnter(IJSCSGlue entering)
        {
            entering.ApplyOnListenable(_ListenerRegister.On);
        }

        private void OnExit(IJSCSGlue exiting, BridgeUpdater context)
        {
            exiting.ApplyOnListenable(_ListenerRegister.Off);

            if ((context == null) || (exiting.Type != JsCsGlueType.Object))
                return;

            _SessionCache.RemoveFromCSharpToJs(exiting);
            context.RequestJSCacheRemove(exiting);

            if (!exiting.CValue.GetType().HasReadWriteProperties())
                return;

            context.RequestUnlisten(exiting.JSValue);
        }

        private void Visit(Action<IJSCSGlue> onChildren)
        {
            GetAllChildren().ForEach(onChildren);
        }

        private void ListenToCSharpChanges() => Visit(OnEnter);
        private void UnlistenToCSharpChanges() => Visit(exiting => OnExit(exiting, null));

        ISet<IJSCSGlue> IUpdatableJSCSGlueCollection.GetAllChildren() => GetAllChildren();
        void IUpdatableJSCSGlueCollection.OnEnter(IJSCSGlue entering) => OnEnter(entering);
        void IUpdatableJSCSGlueCollection.OnExit(IJSCSGlue exiting, BridgeUpdater context) => OnExit(exiting, context);

        private async Task<IJavascriptObject> InjectInHTMLSession(IJSCSGlue root)
        {
            if ( (root == null) || (root.IsBasic()))
                return null;

            if (!_Context.JavascriptFrameworkIsMappingObject)
                return root.JSValue;

            var jvm = _SessionCache.GetMapper(root as IJSCSMappedBridge);
            var res = _sessionInjector.Inject(root.JSValue, jvm);

            if ((root.CValue != null) && (res == null))
            {
                throw ExceptionHelper.GetUnexpected();
            }

            await jvm.UpdateTask;
            return res;
        }

        public async void OnJavaScriptObjectChanges(IJavascriptObject objectchanged, string propertyName, IJavascriptObject newValue)
        {
            try 
            {
                var res = _SessionCache.GetCached(objectchanged) as JsGenericObject;
                if (res == null)
                    return;

                var propertyAccessor = new PropertyAccessor(res.CValue, propertyName, _Logger);
                if (!propertyAccessor.IsSettable) 
                {
                    _Logger.Info(() => $"Unable to set C# from javascript object: property: {propertyName} is readonly.");
                    return;
                }

                var targetType = propertyAccessor.GetTargetType();
                var glue = GetCachedOrCreateBasic(newValue, targetType);
                var bridgeUpdater = glue.IsBasicNotNull() ? null : new BridgeUpdater();

                await Context.RunOnUIContextAsync(() => 
                {
                    using (var relisten = glue.IsBasicNotNull() ? null : ReListen(bridgeUpdater))
                    using (_IsListening ? _ListenerRegister.GetPropertySilenter(res.CValue) : null) 
                    {
                        var oldValue = propertyAccessor.Get();
                        propertyAccessor.Set(glue.CValue);
                        var actualValue = propertyAccessor.Get();

                        if (Object.Equals(actualValue, glue.CValue))
                        {
                            res.UpdateGlueProperty(propertyName, glue);
                            return;
                        }

                        if (!Object.Equals(oldValue, actualValue))
                            CSharpPropertyChanged(res.CValue, new PropertyChangedEventArgs(propertyName));   
                    }
                });

                if (!(bridgeUpdater?.HasUpdatesOnJavascriptContext == true))
                    return;

                await Context.RunOnJavascriptContextAsync(() =>
                {
                    bridgeUpdater.UpdateOnJavascriptContext(Context.ViewModelUpdater);
                });
            }
            catch (Exception e)
            {
                _Logger.Error(() =>$"Unable to update ViewModel from View, exception raised: {e.Message}");
            }
        }

        public async void OnJavaScriptCollectionChanges(JavascriptCollectionChanges changes)
        {
            try
            {
                var res = _SessionCache.GetCached(changes.Collection) as JSArray;
                if (res == null) return;

               var collectionChanges = res.GetChanger(changes, this);

                var updater = new BridgeUpdater();
                await Context.RunOnUIContextAsync(() => 
                {
                    UpdateCollection(res, res.CValue, collectionChanges, updater);
                });

                if (!updater.HasUpdatesOnJavascriptContext)
                    return;

                await Context.RunOnJavascriptContextAsync(() =>
                {
                    updater.UpdateOnJavascriptContext(_Context.ViewModelUpdater);
                });
            }
            catch (Exception e)
            {
                _Logger.Error(() =>$"Unable to update ViewModel from View, exception raised: {e.Message}");
            }
        }

        private void UpdateCollection(JSArray array, object collection, CollectionChanges.CollectionChanges change, BridgeUpdater updater)
        {
            try
            {
                using (ReListen(updater))
                using (_ListenerRegister.GetColllectionSilenter(collection))
                {
                    array.UpdateEventArgsFromJavascript(change);
                }
            }
            catch (Exception e)
            {
                _Logger.Error(() => $"Unable to update ViewModel from View, exception raised: {e.Message}");
            }
        }

        private void CSharpPropertyChanged(object sender, PropertyChangedEventArgs e)
        { 
            var pn = e.PropertyName;
            var propertyAccessor = new PropertyAccessor(sender, pn, _Logger);
            if (!propertyAccessor.IsGettable)
                return;

            var currentfather = _SessionCache.GetCached(sender) as JsGenericObject;
            if (currentfather == null)
                return;

            var nv = propertyAccessor.Get();
            var oldbridgedchild = currentfather.Attributes[pn];

            if (Object.Equals(nv, oldbridgedchild.CValue))
                return;

            UpdateFromCSharpChanges(nv, (child) => currentfather.GetUpdater(pn, child)).DoNotWait();
        }

        private async void CSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await UnsafeCSharpCollectionChanged(sender, e).ConfigureAwait(false);
        }

        private async Task UnsafeCSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var arr = _SessionCache.GetCached(sender) as JSArray;
            if (arr == null)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    await UpdateFromCSharpChanges(e.NewItems[0], (addvalue) => arr.GetAddUpdater(addvalue, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    await UpdateFromCSharpChanges(e.NewItems[0], (newvalue) => arr.GetReplaceUpdater(newvalue, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    await UpdateFromCSharpChanges(() => arr.GetRemoveUpdater(e.OldStartingIndex), true);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    await UpdateFromCSharpChanges(() => arr.GetResetUpdater(), true);
                    break;

                case NotifyCollectionChangedAction.Move:
                    await UpdateFromCSharpChanges(() => arr.GetMoveUpdater(e.OldStartingIndex, e.NewStartingIndex), false);
                    break;
            }
        }

        public Task<IJSCSGlue> RegisterInSession(object nv)
        {
            return UpdateFromCSharpChanges(nv, GetUnrootedEntitiesUpdater);
        }

        private BridgeUpdater GetUnrootedEntitiesUpdater(IJSCSGlue newbridgedchild)
        {
            _UnrootedEntities.Add(newbridgedchild);
            return new BridgeUpdater();
        }

        private Task UpdateFromCSharpChanges(Func<BridgeUpdater> updaterBuilder, bool needToCheckListener)
        {
            CheckUIContext();

            BridgeUpdater updater = null;
            using (var relisten = needToCheckListener ? ReListen(): null)
            {
                updater = Update(updaterBuilder, relisten);
            }

            if (!updater.HasUpdatesOnJavascriptContext)
                return TaskHelper.Ended();

            return RunInJavascriptContext(() =>
            {
                updater.UpdateOnJavascriptContext(_Context.ViewModelUpdater);
            });
        }

        private Task<IJSCSGlue> UpdateFromCSharpChanges(object newCSharpObject, Func<IJSCSGlue, BridgeUpdater> updaterBuilder)
        {
            CheckUIContext();

            var value = _JSObjectBuilder.Map(newCSharpObject);
            if (value == null)
                return null;               

            BridgeUpdater updater = null;
            using (var relisten = value.IsBasicNotNull()? null : ReListen())
            {
                updater = Update(() => updaterBuilder(value), relisten);
            }

            if (!_IsLoaded)
                return Task.FromResult(value);

            return RunInJavascriptContext(async () =>
            {
                _JavascriptObjectBuilder.UpdateJavascriptValue(value);

                await InjectInHTMLSession(value);

                updater.UpdateOnJavascriptContext(_Context.ViewModelUpdater);
                return value;
            });
        }

        private BridgeUpdater Update(Func<BridgeUpdater> updaterBuilder, IExitContext relisten)
        {
            var updater = updaterBuilder();
            updater.Cache = _SessionCache;
            relisten?.SetBridgeUpdater(updater);
            return updater;
        }

        private void CheckUIContext()
        {
            if (!_Context.UIDispatcher.IsInContext())
                throw ExceptionHelper.Get("MVVM ViewModel should be updated from UI thread. Use await pattern and Dispatcher to do so.");
        }

        private IExitContext ReListen(BridgeUpdater updater=null) => new ReListener(this, updater);

        public IJSCSGlue GetCachedOrCreateBasic(IJavascriptObject javascriptObject, Type targetType)
        {
            if (javascriptObject == null)
                return null;

            object targetvalue;
            if (_Context.WebView.Converter.GetSimpleValue(javascriptObject, out targetvalue, targetType))
            {
                return new JSBasicObject(javascriptObject, targetvalue);
            }

            //Use local and local cache for objet not created in javascript session such as enum
            var res = _SessionCache.GetCached(javascriptObject);
            if (res != null)
                return res;

            var message = $"Unable to convert javascript object: {javascriptObject} to C# session. Value will be default to null. Please check javascript bindings.";
            _Logger.Info(message);
            throw new ArgumentException(message);
        }
    }
}
