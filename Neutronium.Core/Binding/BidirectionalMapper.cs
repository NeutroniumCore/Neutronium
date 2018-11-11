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
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.GlueObject.Basic;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding
{
    public class BidirectionalMapper : IDisposable, IJavascriptToCSharpConverter, IJavascriptChangesObserver
    {
        private readonly IWebSessionLogger _Logger;
        private readonly CSharpToJavascriptConverter _JsObjectBuilder;
        private readonly IJavascriptObjectBuilderStrategyFactory _BuilderStrategyFactory;
        private readonly FullListenerRegister _ListenerRegister;
        private readonly List<IJsCsGlue> _UnrootedEntities = new List<IJsCsGlue>();
        private readonly SessionCacher _SessionCache;
        private List<Action> _UpdatesToBeReplayed;

        private IJavascriptObjectBuilderStrategy _BuilderStrategy;
        private IJavascriptSessionInjector _SessionInjector;
        private bool _IsLoaded = false;

        public IJsCsGlue JsValueRoot { get; private set; }
        public bool ListenToCSharp => (Mode != JavascriptBindingMode.OneTime);
        public JavascriptBindingMode Mode { get; }
        public HtmlViewContext Context { get; }

        private readonly object _RootObject;

        internal BidirectionalMapper(object root, HtmlViewEngine contextBuilder, JavascriptBindingMode mode, IWebSessionLogger logger, IJavascriptObjectBuilderStrategyFactory strategyFactory) :
            this(root, contextBuilder, null, strategyFactory, mode, logger)
        {
        }

        internal BidirectionalMapper(object root, HtmlViewEngine contextBuilder, IGlueFactory glueFactory,
            IJavascriptObjectBuilderStrategyFactory strategyFactory, JavascriptBindingMode mode, IWebSessionLogger logger)
        {
            _BuilderStrategyFactory = strategyFactory ?? new StandardStrategyFactory();
            Mode = mode;
            _Logger = logger;
            var javascriptObjecChanges = (mode == JavascriptBindingMode.TwoWay) ? (IJavascriptChangesObserver)this : null;
            Context = contextBuilder.GetMainContext(javascriptObjecChanges);
            _SessionCache = new SessionCacher();
            _ListenerRegister = ListenToCSharp ? new FullListenerRegister(OnCSharpPropertyChanged, OnCSharpCollectionChanged): null;
            glueFactory = glueFactory ?? GlueFactoryFactory.GetFactory(Context, _SessionCache, this, _ListenerRegister?.On);
            _JsObjectBuilder = new CSharpToJavascriptConverter(_SessionCache, glueFactory, _Logger);
            _RootObject = root;
        }

        internal async Task MapRootVm()
        {
            await Context.RunOnUiContextAsync(() =>
            {
                JsValueRoot = _JsObjectBuilder.Map(_RootObject);
                JsValueRoot.AddRef();
            });
        }

        internal async Task UpdateJavascriptObjects(bool debugMode)
        {
            CheckUiContext();

            await RunInJavascriptContext(async () =>
            {
                RegisterJavascriptHelper();

                Context.InitOnJsContext(debugMode);
                _SessionInjector = Context.JavascriptSessionInjector;

                _BuilderStrategy = _BuilderStrategyFactory.GetStrategy(Context.WebView, _SessionCache, Context.JavascriptFrameworkIsMappingObject);
                _BuilderStrategy.UpdateJavascriptValue(JsValueRoot);

                IJavascriptObject res;
                if (Context.JavascriptFrameworkIsMappingObject)
                    res = await InjectInHtmlSession(JsValueRoot);
                else
                    res = JsValueRoot.JsValue;

                await _SessionInjector.RegisterMainViewModel(res);
            });

            _IsLoaded = true;

            _UpdatesToBeReplayed?.ForEach(up => up());
            _UpdatesToBeReplayed = null;
        }

        private void RegisterJavascriptHelper()
        {
            var resource = new ResourceReader("scripts", this);
            var infa = resource.Load("Infra.js")
                .Replace(NeutroniumConstants.ReadOnlyFlagTemplate, NeutroniumConstants.ReadOnlyFlag)
                .Replace("{{ReadOnly}}", ((int)ObjectObservability.ReadOnly).ToString());

            Context.WebView.ExecuteJavaScript(infa);
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
            _BuilderStrategy?.Dispose();
        }

        private void OnExit(IJsCsGlue exiting)
        {
            exiting.ApplyOnListenable(_ListenerRegister.Off);
            exiting.GetJsSessionValue().Dispose();
        }

        private void UnlistenToCSharpChanges()
        {
            _SessionCache.AllElements.ForEach(OnExit);
        }

        private async Task<IJavascriptObject> InjectInHtmlSession(IJsCsGlue root)
        {
            if (!Context.JavascriptFrameworkIsMappingObject)
                return root?.JsValue;

            if ((root == null) || (root.IsBasic()))
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
                if (!(_SessionCache.GetCached(objectchanged) is JsGenericObject currentfather))
                    return;

                var propertyUpdater = currentfather.GetPropertyUpdater(propertyName);
                if (!propertyUpdater.IsSettable)
                {
                    LogReadOnlyProperty(propertyName);
                    return;
                }

                var targetType = propertyUpdater.TargetType;
                var glue = GetCachedOrCreateBasic(newValue, targetType);
                var bridgeUpdater = new BridgeUpdater(_SessionCache);

                await Context.RunOnUiContextAsync(() =>
                {
                    using (_ListenerRegister.GetPropertySilenter(currentfather.CValue, propertyName))
                    {
                        var oldValue = propertyUpdater.GetCurrentChildValue();

                        try
                        {
                            propertyUpdater.Set(glue.CValue);
                        }
                        catch (Exception exception)
                        {
                            LogSetError(propertyName, targetType, glue.CValue, exception);
                        }

                        var actualValue = propertyUpdater.GetCurrentChildValue();

                        if (Equals(actualValue, glue.CValue))
                        {
                            var old = currentfather.UpdateGlueProperty(propertyUpdater, glue);
                            bridgeUpdater.Remove(old);
                            bridgeUpdater.CleanAfterChangesOnUiThread(_ListenerRegister.Off);
                            return;
                        }

                        if (!Equals(oldValue, actualValue))
                        {
                            OnCSharpPropertyChanged(currentfather.CValue, new PropertyChangedEventArgs(propertyName));
                        }                        
                    }
                });

                if (!bridgeUpdater.HasUpdatesOnJavascriptContext)
                    return;

                await Context.RunOnJavascriptContextAsync(() =>
                {
                    bridgeUpdater.UpdateOnJavascriptContext(Context.ViewModelUpdater);
                });
            }
            catch (Exception exception)
            {
                LogJavascriptSetException(exception);
            }
        }

        private void LogSetError(string propertyName, Type targetType, object @object, Exception exception)
        {
            _Logger.Info($"Unable to set C# from javascript object: property: {propertyName} of {targetType}, javascript value {@object}. Exception {exception} was thrown.");
        }

        private void LogReadOnlyProperty(string propertyName) 
        {
            _Logger.Info(() => $"Unable to set C# from javascript object: property: {propertyName} is readonly.");
        }

        private void LogJavascriptSetException(Exception exception) 
        {
            _Logger.Error(() => $"Unable to update ViewModel from View, exception raised: {exception.Message}");
        }

        public async void OnJavaScriptCollectionChanges(JavascriptCollectionChanges changes)
        {
            try
            {
                if (!(_SessionCache.GetCached(changes.Collection) is JsArray res))
                    return;

                var collectionChanges = res.GetChanger(changes, this);

                var updater = new BridgeUpdater(_SessionCache);
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
            catch (Exception exception)
            {
                LogJavascriptSetException(exception);
            }
        }

        private void UpdateCollection(JsArray array, object collection, CollectionChanges.CollectionChanges change, BridgeUpdater updater)
        {
            try
            {
                using (_ListenerRegister.GetColllectionSilenter(collection))
                {
                    array.UpdateEventArgsFromJavascript(change, updater);
                }
                updater.CleanAfterChangesOnUiThread(_ListenerRegister.Off);
            }
            catch (Exception exception)
            {
                LogJavascriptSetException(exception);
            }
        }

        private void OnCSharpPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(_SessionCache.GetCached(sender) is JsGenericObject currentfather))
                return;

            var propertyUpdater = currentfather.GetPropertyUpdater(e.PropertyName);
            if (!propertyUpdater.IsValid)
                return;

            var newValue = propertyUpdater.GetCurrentChildValue();
            if (!propertyUpdater.HasChanged(newValue))
                return;

            UpdateFromCSharpChanges(newValue, (child) => currentfather.GetUpdater(propertyUpdater, child));
        }

        private void OnCSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsafeCSharpCollectionChanged(sender, e);
        }

        private void UnsafeCSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!(_SessionCache.GetCached(sender) is JsArray arr))
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
                    UpdateFromCSharpChanges(() => arr.GetRemoveUpdater(e.OldStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Reset:
                    UpdateFromCSharpChanges(() => arr.GetResetUpdater());
                    break;

                case NotifyCollectionChangedAction.Move:
                    UpdateFromCSharpChanges(() => arr.GetMoveUpdater(e.OldStartingIndex, e.NewStartingIndex));
                    break;
            }
        }

        public void RegisterInSession(object nv, Action<IJsCsGlue> performAfterBuild)
        {
            UpdateFromCSharpChanges(nv, (bridge) => GetUnrootedEntitiesUpdater(bridge, performAfterBuild));
        }

        private BridgeUpdater GetUnrootedEntitiesUpdater(IJsCsGlue newbridgedchild, Action<IJsCsGlue> performAfterBuild)
        {
            _UnrootedEntities.Add(newbridgedchild.AddRef());
            return new BridgeUpdater(updater => 
            {
                updater.InjectDetached(newbridgedchild.GetJsSessionValue());
                performAfterBuild(newbridgedchild);
            });
        }

        private void UpdateFromCSharpChanges(Func<BridgeUpdater> updaterBuilder)
        {
            CheckUiContext();

            var updater = Update(_ => updaterBuilder(), null);
            updater.CleanAfterChangesOnUiThread(_ListenerRegister.Off);

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

            if (!_IsLoaded) 
            {
                //Changes happening on the UI thread while javascript thread is still initializing bindings
                //We keep the updates here to be replayed when binding is done
                _UpdatesToBeReplayed = _UpdatesToBeReplayed ?? new List<Action>();
                _UpdatesToBeReplayed.Add(() => UpdateFromCSharpChanges(newCSharpObject, updaterBuilder));
                return;
            }

            var value = _JsObjectBuilder.Map(newCSharpObject);
            if (value == null)
                return;

            var updater = Update(updaterBuilder, value);
            updater.CleanAfterChangesOnUiThread(_ListenerRegister.Off);
            UpdateOnJavascriptContextAllContext(updater, value);
        }

        private void UpdateOnJavascriptContextAllContext(BridgeUpdater updater, IJsCsGlue value)
        {
            if (Context.JavascriptFrameworkIsMappingObject) 
            {
                DispatchInJavascriptContext(() => UpdateOnJavascriptContextWithMapping(updater, value));
                return;
            }

            DispatchInJavascriptContext(() => UpdateOnJavascriptContextWithoutMapping(updater, value));
        }

        private void UpdateOnJavascriptContextWithoutMapping(BridgeUpdater updater, IJsCsGlue value)
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

        private BridgeUpdater Update(Func<IJsCsGlue, BridgeUpdater> updaterBuilder, IJsCsGlue glue)
        {
            var updater = updaterBuilder(glue);
            updater.Cache = _SessionCache;
            return updater;
        }

        private void CheckUiContext()
        {
            if (!Context.UiDispatcher.IsInContext())
                throw ExceptionHelper.Get("MVVM ViewModel should be updated from UI thread. Use await pattern and Dispatcher to do so.");
        }

        public IJsCsGlue GetCachedOrCreateBasic(IJavascriptObject javascriptObject, Type targetType)
        {
            if (javascriptObject == null)
                return null;

            if (Context.WebView.Converter.GetSimpleValue(javascriptObject, out var targetvalue, targetType)) {
                return new JsBasicObject(javascriptObject, targetvalue);
            }

            if (targetType?.IsEnum == true)
            {
                var intValue = javascriptObject.GetValue("intValue")?.GetIntValue();
                if (!intValue.HasValue)
                    return null;

                targetvalue = Enum.ToObject(targetType, intValue.Value);
                return new JsEnum(javascriptObject, (Enum)targetvalue);
            }

            var res = _SessionCache.GetCached(javascriptObject);
            if (res != null)
                return res;

            var message = $"Unable to convert javascript object: {javascriptObject} to C# session. Value will be default to null. Please check javascript bindings.";
            _Logger.Info(message);
            throw new ArgumentException(message);
        }
    }
}
