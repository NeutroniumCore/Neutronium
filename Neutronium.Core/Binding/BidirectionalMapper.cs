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
    public class BidirectionalMapper : IDisposable, IUpdatableJsCsGlueCollection, IJavascriptToCSharpConverter, IJavascriptChangesObserver
    {
        private readonly IWebSessionLogger _Logger;
        private readonly CSharpToJavascriptConverter _JsObjectBuilder;
        private readonly IJavascriptObjectBuilderStrategyFactory _BuilderStrategyFactory;
        private IJavascriptObjectBuilderStrategy _BuilderStrategy;
        private IJavascriptSessionInjector _SessionInjector;
        private readonly SessionCacher _SessionCache;
        private IJsCsGlue _Root;
        private readonly FullListenerRegister _ListenerRegister;
        private readonly List<IJsCsGlue> _UnrootedEntities= new List<IJsCsGlue>();
        private bool _IsListening = false;
        private bool _IsLoaded = false;

        public IJsCsGlue JsValueRoot => _Root;
        public bool ListenToCSharp => (Mode != JavascriptBindingMode.OneTime);
        public JavascriptBindingMode Mode { get; }
        public HtmlViewContext Context { get; }

        private readonly object _RootObject;

        internal BidirectionalMapper(object root, HtmlViewEngine contextBuilder, JavascriptBindingMode mode, IWebSessionLogger logger, IJavascriptObjectBuilderStrategyFactory strategyFactory) :
            this(root, contextBuilder, null, strategyFactory,  mode, logger)
        {        
        }

        internal BidirectionalMapper(object root, HtmlViewEngine contextBuilder, IGlueFactory glueFactory,
            IJavascriptObjectBuilderStrategyFactory strategyFactory, JavascriptBindingMode mode, IWebSessionLogger logger)
        {
            _BuilderStrategyFactory = strategyFactory?? new StandardStrategyFactory();
            Mode = mode;
            _Logger = logger;
            var javascriptObjecChanges = (mode == JavascriptBindingMode.TwoWay) ? (IJavascriptChangesObserver)this : null;
            Context = contextBuilder.GetMainContext(javascriptObjecChanges);
            _SessionCache = new SessionCacher();
            _ListenerRegister = new FullListenerRegister(
                                        (n) => n.PropertyChanged += CSharpPropertyChanged,
                                        (n) => n.PropertyChanged -= CSharpPropertyChanged,
                                        (n) => n.CollectionChanged += CSharpCollectionChanged,
                                        (n) => n.CollectionChanged -= CSharpCollectionChanged,
                                        (c) => c.ListenChanges(),
                                        (c) => c.UnListenChanges());
            glueFactory = glueFactory ?? GlueFactoryFactory.GetFactory(Context, _SessionCache, this);
            _JsObjectBuilder = new CSharpToJavascriptConverter(contextBuilder.HtmlWindow, _SessionCache, glueFactory, _Logger);
            _RootObject = root;
        }

        internal async Task IntrospectVm(object addicionalObject) 
        {
            await Context.RunOnUiContextAsync(() => 
            {
                _Root = _JsObjectBuilder.Map(_RootObject, addicionalObject);

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

                Context.InitOnJsContext(debugMode);
                _SessionInjector = Context.JavascriptSessionInjector;

                _BuilderStrategy = _BuilderStrategyFactory.GetStrategy(Context.WebView, _SessionCache, Context.JavascriptFrameworkIsMappingObject);
                _BuilderStrategy.UpdateJavascriptValue(_Root);

                IJavascriptObject res;
                if (Context.JavascriptFrameworkIsMappingObject)
                    res = await InjectInHtmlSession(_Root);
                else
                    res = _Root.JsValue;

                await _SessionInjector.RegisterMainViewModel(res);

                _IsLoaded = true;
            });
        }

        private void RegisterJavascriptHelper()
        {
            var resource = new ResourceReader("scripts", this);
            Context.WebView.ExecuteJavaScript(resource.Load("Infra.js"));
        }

        private void DispatchInJavascriptContext(Action run)
        {
            Context.WebView.Dispatch(run);
        }

        private Task RunInJavascriptContext(Func<Task> run)
        {
            return Context.WebView.Evaluate(run);
        }

        private void DispatchInJavascriptContext(Func<Task> run)
        {
            Context.WebView.Dispatch(() => run());
        }

        public void Dispose()
        {
            if (ListenToCSharp)
                UnlistenToCSharpChanges();

            Context.Dispose();
            _UnrootedEntities.Clear();
            _Logger.Debug("BidirectionalMapper disposed");
        }

        private ISet<IJsCsGlue> GetAllChildren()
        {
            var @glues = _Root.GetAllChildren(FilterBasic);
            @glues.UnionWith(_UnrootedEntities.SelectMany(ent => ent.GetAllChildren(FilterBasic)));
            return @glues;
        }

        public void CollectionChanged()
        {
        }

        private static bool FilterBasic(JsCsGlueType type) => type == JsCsGlueType.Basic;

        private void OnEnter(IJsCsGlue entering)
        {
            entering.ApplyOnListenable(_ListenerRegister.On);
        }

        private void OnExit(IJsCsGlue exiting)
        {
            exiting.ApplyOnListenable(_ListenerRegister.Off);
        }

        private void OnExit(IJsCsGlue exiting, BridgeUpdater context)
        {
            exiting.ApplyOnListenable(_ListenerRegister.Off);

            if (exiting.Type != JsCsGlueType.Object)
                return;

            _SessionCache.RemoveFromCSharpToJs(exiting);
            context.RequestJsCacheRemove(exiting);

            if (!exiting.CValue.GetType().HasReadWriteProperties())
                return;

            context.RequestUnlisten(exiting.JsValue);
        }

        private void Visit(Action<IJsCsGlue> onChildren)
        {
            _SessionCache.AllElements.ForEach(onChildren);
        }

        private void ListenToCSharpChanges() => Visit(OnEnter);
        private void UnlistenToCSharpChanges() => Visit(OnExit);

        ISet<IJsCsGlue> IUpdatableJsCsGlueCollection.GetAllChildren() => GetAllChildren();
        void IUpdatableJsCsGlueCollection.OnEnter(IJsCsGlue entering) => OnEnter(entering);
        void IUpdatableJsCsGlueCollection.OnExit(IJsCsGlue exiting, BridgeUpdater context) => OnExit(exiting, context);

        private async Task<IJavascriptObject> InjectInHtmlSession(IJsCsGlue root)
        {
            if (!Context.JavascriptFrameworkIsMappingObject)
                return root?.JsValue;

            if ( (root == null) || (root.IsBasic()))
                return null;

            var jvm = _SessionCache.GetMapper(root as IJsCsMappedBridge);
            var res = _SessionInjector.Inject(root.JsValue, jvm);

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

                var fatherValue = res.CValue;
                var propertyAccessor = fatherValue.GetType().GetReadProperty(propertyName);

                if (propertyAccessor?.IsSettable != true)
                {
                    _Logger.Info(() => $"Unable to set C# from javascript object: property: {propertyName} is readonly.");
                    return;
                }

                var targetType = propertyAccessor.GetTargetType();
                var glue = GetCachedOrCreateBasic(newValue, targetType);
                var bridgeUpdater = glue.IsBasicNotNull() ? null : new BridgeUpdater();

                await Context.RunOnUiContextAsync(() =>
                {
                    using (glue.IsBasicNotNull() ? null : ReListen(bridgeUpdater))
                    using (_IsListening ? _ListenerRegister.GetPropertySilenter(res.CValue) : null)
                    {
                        var oldValue = propertyAccessor.Get(fatherValue);

                        try
                        {
                            propertyAccessor.Set(fatherValue, glue.CValue);
                        }
                        catch (Exception e)
                        {
                            _Logger.Info($"Unable to set C# from javascript object: property: {propertyName} of {targetType}, javascript value {glue.CValue}. Exception {e} was thrown.");
                        }

                        var actualValue = propertyAccessor.Get(fatherValue);

                        if (Object.Equals(actualValue, glue.CValue))
                        {
                            res.UpdateGlueProperty(propertyName, glue);
                            return;
                        }

                        if (!Object.Equals(oldValue, actualValue))
                            CSharpPropertyChanged(res.CValue, new PropertyChangedEventArgs(propertyName));
                    }
                });

                if (bridgeUpdater?.HasUpdatesOnJavascriptContext != true)
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
                var res = _SessionCache.GetCached(changes.Collection) as JsArray;
                if (res == null) return;

               var collectionChanges = res.GetChanger(changes, this);

                var updater = new BridgeUpdater();
                await Context.RunOnUiContextAsync(() => 
                {
                    UpdateCollection(res, res.CValue, collectionChanges, updater);
                });

                if (!updater.HasUpdatesOnJavascriptContext)
                    return;

                await Context.RunOnJavascriptContextAsync(() =>
                {
                    updater.UpdateOnJavascriptContext(Context.ViewModelUpdater);
                });
            }
            catch (Exception e)
            {
                _Logger.Error(() =>$"Unable to update ViewModel from View, exception raised: {e.Message}");
            }
        }

        private void UpdateCollection(JsArray array, object collection, CollectionChanges.CollectionChanges change, BridgeUpdater updater)
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
            var propertyName = e.PropertyName;

            var propertyAccessor = sender.GetType().GetReadProperty(propertyName);
            if (propertyAccessor == null)
                return;

            var currentfather = _SessionCache.GetCached(sender) as JsGenericObject;
            if (currentfather == null)
                return;

            var nv = propertyAccessor.Get(sender);
            var oldbridgedchild = currentfather.GetAttribute(propertyName);

            if (Object.Equals(nv, oldbridgedchild.CValue))
                return;

            UpdateFromCSharpChanges(nv, (child) => currentfather.GetUpdater(propertyName, child));
        }

        private void CSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsafeCSharpCollectionChanged(sender, e);
        }

        private void UnsafeCSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var arr = _SessionCache.GetCached(sender) as JsArray;
            if (arr == null)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    UpdateFromCSharpChanges(e.NewItems[0], (addvalue) => arr.GetAddUpdater(addvalue, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    UpdateFromCSharpChanges(e.NewItems[0], (newvalue) => arr.GetReplaceUpdater(newvalue, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    UpdateFromCSharpChanges(() => arr.GetRemoveUpdater(e.OldStartingIndex), true);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    UpdateFromCSharpChanges(() => arr.GetResetUpdater(), true);
                    break;

                case NotifyCollectionChangedAction.Move:
                    UpdateFromCSharpChanges(() => arr.GetMoveUpdater(e.OldStartingIndex, e.NewStartingIndex), false);
                    break;
            }
        }

        public void RegisterInSession(object nv, Action<IJsCsGlue> performAfterBuild)
        {
            UpdateFromCSharpChanges(nv, (bridge) => GetUnrootedEntitiesUpdater(bridge, performAfterBuild));
        }

        private BridgeUpdater GetUnrootedEntitiesUpdater(IJsCsGlue newbridgedchild, Action<IJsCsGlue> performAfterBuild)
        {
            _UnrootedEntities.Add(newbridgedchild);
            return new BridgeUpdater(_ => performAfterBuild(newbridgedchild));
        }

        private void UpdateFromCSharpChanges(Func<BridgeUpdater> updaterBuilder, bool needToCheckListener)
        {
            CheckUiContext();

            BridgeUpdater updater = null;
            using (var relisten = needToCheckListener ? ReListen(): null)
            {
                updater = Update(updaterBuilder, relisten);
            }

            if (!updater.HasUpdatesOnJavascriptContext)
                return;

            DispatchInJavascriptContext(() =>
            {
                updater.UpdateOnJavascriptContext(Context.ViewModelUpdater);
            });
        }

        private void UpdateFromCSharpChanges(object newCSharpObject, Func<IJsCsGlue, BridgeUpdater> updaterBuilder)
        {
            CheckUiContext();

            var value = _JsObjectBuilder.Map(newCSharpObject);
            if (value == null)
                return;               

            BridgeUpdater updater;
            using (var relisten = value.IsBasicNotNull()? null : ReListen())
            {
                updater = Update(() => updaterBuilder(value), relisten);
            }

            if (!_IsLoaded)
                return;

            if (Context.JavascriptFrameworkIsMappingObject)
            {
                DispatchInJavascriptContext(() => UpdateOnJavascriptContextWithMapping(updater, value));
                return;
            }

            DispatchInJavascriptContext(() => UpdateOnJavascriptContext(updater, value));
        }

        private void UpdateOnJavascriptContext(BridgeUpdater updater, IJsCsGlue value)
        {
            _BuilderStrategy.UpdateJavascriptValue(value);
            updater.UpdateOnJavascriptContext(Context.ViewModelUpdater);
        }

        private async Task UpdateOnJavascriptContextWithMapping(BridgeUpdater updater, IJsCsGlue value)
        {
            _BuilderStrategy.UpdateJavascriptValue(value);
            await InjectInHtmlSession(value);
            updater.UpdateOnJavascriptContext(Context.ViewModelUpdater);
        }

        private BridgeUpdater Update(Func<BridgeUpdater> updaterBuilder, IExitContext relisten)
        {
            var updater = updaterBuilder();
            updater.Cache = _SessionCache;
            relisten?.SetBridgeUpdater(updater);
            return updater;
        }

        private void CheckUiContext()
        {
            if (!Context.UiDispatcher.IsInContext())
                throw ExceptionHelper.Get("MVVM ViewModel should be updated from UI thread. Use await pattern and Dispatcher to do so.");
        }

        private IExitContext ReListen(BridgeUpdater updater=null) => new ReListener(this, updater);

        public IJsCsGlue GetCachedOrCreateBasic(IJavascriptObject javascriptObject, Type targetType)
        {
            if (javascriptObject == null)
                return null;

            object targetvalue;
            if (Context.WebView.Converter.GetSimpleValue(javascriptObject, out targetvalue, targetType))
            {
                return new JsBasicObject(javascriptObject, targetvalue);
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
