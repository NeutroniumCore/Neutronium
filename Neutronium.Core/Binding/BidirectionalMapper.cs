using System;
using System.Collections.Generic;
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
using Neutronium.Core.Binding.Updaters;

namespace Neutronium.Core.Binding
{
    public class BidirectionalMapper : IDisposable, IJavascriptToCSharpConverter, IJavascriptChangesObserver, ISessionMapper
    {
        private readonly IWebSessionLogger _Logger;
        private readonly CSharpToJavascriptConverter _JsObjectBuilder;
        private readonly IJavascriptObjectBuilderStrategyFactory _BuilderStrategyFactory;
        private readonly ListenerUpdater _ListenerUpdater;
        private readonly List<IJsCsGlue> _UnrootedEntities = new List<IJsCsGlue>();
        private readonly SessionCacher _SessionCache;
        private readonly IJsUpdateHelper _JsUpdateHelper;

        private IJavascriptObjectBuilderStrategy _BuilderStrategy;
        private IJavascriptSessionInjector _SessionInjector;

        public IJsCsGlue JsValueRoot { get; private set; }
        public bool ListenToCSharp => (Mode != JavascriptBindingMode.OneTime);
        public JavascriptBindingMode Mode { get; }
        public HtmlViewContext Context { get; }

        private readonly object _RootObject;

        internal BidirectionalMapper(object root, HtmlViewEngine engine, JavascriptBindingMode mode, IWebSessionLogger logger, IJavascriptObjectBuilderStrategyFactory strategyFactory) :
            this(root, engine, null, strategyFactory, mode, logger, null)
        {
        }

        internal BidirectionalMapper(object root, HtmlViewEngine contextBuilder, IGlueFactory glueFactory,
            IJavascriptObjectBuilderStrategyFactory strategyFactory, JavascriptBindingMode mode, IWebSessionLogger logger, SessionCacher sessionCacher)
        {
            _BuilderStrategyFactory = strategyFactory ?? new StandardStrategyFactory();
            Mode = mode;
            _Logger = logger;
            var javascriptObjectChanges = (mode == JavascriptBindingMode.TwoWay) ? (IJavascriptChangesObserver)this : null;
            Context = contextBuilder.GetMainContext(javascriptObjectChanges);
            _SessionCache = sessionCacher ?? new SessionCacher();
            var jsUpdateHelper = new JsUpdateHelper(this, Context, () => _BuilderStrategy, _SessionCache);
            _ListenerUpdater = ListenToCSharp ? new ListenerUpdater(jsUpdateHelper) : null;
            glueFactory = glueFactory ?? GlueFactoryFactory.GetFactory(Context, _SessionCache, this, _ListenerUpdater?.On);
            _JsObjectBuilder = new CSharpToJavascriptConverter(_SessionCache, glueFactory, _Logger);
            jsUpdateHelper.JsObjectBuilder =  _JsObjectBuilder;
            _JsUpdateHelper = jsUpdateHelper;
            _RootObject = root;

            _JsUpdateHelper.CheckUiContext();
            JsValueRoot = _JsObjectBuilder.Map(_RootObject);
            JsValueRoot.AddRef();
        }

        internal Task UpdateJavascriptObjects(bool debugMode)
        {
            return Context.JavascriptFrameworkIsMappingObject ? 
                RunInJavascriptContext(() => InitWithMapping(debugMode)) : 
                RunInJavascriptContext(() => InitWithoutMapping(debugMode));
        }

        private Task InitWithoutMapping(bool debugMode)
        {
            InitOnJavascriptContext(debugMode);
            return RegisterMain(JsValueRoot.JsValue);
        }

        private async Task InitWithMapping(bool debugMode)
        {
            InitOnJavascriptContext(debugMode);
            var res = await InjectInHtmlSession(JsValueRoot);
            await RegisterMain(res);
        }

        private void InitOnJavascriptContext(bool debugMode)
        {
            RegisterJavascriptHelper();
            Context.InitOnJsContext(debugMode);
            _SessionInjector = Context.JavascriptSessionInjector;
            _BuilderStrategy = _BuilderStrategyFactory.GetStrategy(Context.WebView, _SessionCache, Context.JavascriptFrameworkIsMappingObject);
            _BuilderStrategy.UpdateJavascriptValue(JsValueRoot);
        }

        private Task RegisterMain(IJavascriptObject res)
        {
            var registerTask = _SessionInjector.RegisterMainViewModel(res);
            _JsUpdateHelper.DispatchInUiContext(JavascriptReady);
            return registerTask;
        }

        private void JavascriptReady()
        {
            _OnJavascriptSessionReady?.Invoke(this, EventArgs.Empty);
        }

        private void RegisterJavascriptHelper()
        {
            var resource = new ResourceReader("scripts", this);
            var infra = resource.Load("Infra.js")
                .Replace(NeutroniumConstants.ReadOnlyFlagTemplate, NeutroniumConstants.ReadOnlyFlag)
                .Replace("{{ReadOnly}}", ((int)ObjectObservability.ReadOnly).ToString());

            Context.WebView.ExecuteJavaScript(infra);
        }

        private Task RunInJavascriptContext(Func<Task> run)
        {
            return Context.WebView.Evaluate(run);
        }

        public void Dispose()
        {
            _JsUpdateHelper.CheckUiContext();
            if (ListenToCSharp)
                OnAllJsGlues(UnlistenGlue);

            Context.Dispose();
            _UnrootedEntities.Clear();
            _Logger.Debug("BidirectionalMapper disposed");
            _BuilderStrategy?.Dispose();
        }

        private void UnlistenGlue(IJsCsGlue exiting)
        {
            exiting.ApplyOnListenable(_ListenerUpdater.Off);
        }

        private void OnAllJsGlues(Action<IJsCsGlue> @do)
        {
            _SessionCache.AllElementsUiContext.ForEach(@do);
        }

        Task<IJavascriptObject> ISessionMapper.InjectInHtmlSession(IJsCsGlue root) => InjectInHtmlSession(root);

        private event EventHandler<EventArgs> _OnJavascriptSessionReady;
        event EventHandler<EventArgs> ISessionMapper.OnJavascriptSessionReady
        {
            add => _OnJavascriptSessionReady += value;
            remove => _OnJavascriptSessionReady -= value;
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

        public async void OnJavaScriptObjectChanges(IJavascriptObject objectChanged, string propertyName, IJavascriptObject newValue)
        {
            try
            {
                if (!(_SessionCache.GetCached(objectChanged) is JsGenericObject currentFather))
                    return;

                var propertyUpdater = currentFather.GetPropertyUpdater(propertyName);
                if (!propertyUpdater.IsSettable)
                {
                    LogReadOnlyProperty(propertyName);
                    return;
                }

                var glue = GetCachedOrCreateBasic(newValue, propertyUpdater.TargetType);
                var bridgeUpdater = await Context.EvaluateOnUiContextAsync(() => UpdateOnUiContextChangeFromJs(propertyUpdater, glue));

                if (bridgeUpdater?.HasUpdatesOnJavascriptContext != true)
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

        private BridgeUpdater UpdateOnUiContextChangeFromJs(AttibuteUpdater propertyUpdater, IJsCsGlue glue)
        {
            var currentFather = propertyUpdater.Father;
            using (_ListenerUpdater.GetPropertySilenter(currentFather.CValue, propertyUpdater.PropertyName))
            {
                var oldValue = propertyUpdater.GetCurrentChildValue();

                try
                {
                    propertyUpdater.Set(glue.CValue);
                }
                catch (Exception exception)
                {
                    LogSetError(propertyUpdater.PropertyName, propertyUpdater.TargetType, glue.CValue, exception);
                }

                var actualValue = propertyUpdater.GetCurrentChildValue();

                if (Equals(actualValue, glue.CValue))
                {
                    var bridgeUpdater = new BridgeUpdater(_SessionCache);
                    var old = currentFather.UpdateGlueProperty(propertyUpdater, glue);
                    bridgeUpdater.CheckForRemove(old)
                        .CleanAfterChangesOnUiThread(_ListenerUpdater.Off);
                    return bridgeUpdater;
                }

                if (!Equals(oldValue, actualValue))
                {
                    _ListenerUpdater.OnCSharpPropertyChanged(currentFather.CValue, propertyUpdater.PropertyName);
                }

                return null;
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
                using (_ListenerUpdater.GetColllectionSilenter(collection))
                {
                    array.UpdateEventArgsFromJavascript(change, updater);
                }
                updater.CleanAfterChangesOnUiThread(_ListenerUpdater.Off);
            }
            catch (Exception exception)
            {
                LogJavascriptSetException(exception);
            }
        }

        private BridgeUpdater GetUnrootedEntitiesUpdater(IJsCsGlue newBridgeChild, Action<IJsCsGlue> performAfterBuild)
        {
            _UnrootedEntities.Add(newBridgeChild.AddRef());
            return new BridgeUpdater(updater => 
            {
                updater.InjectDetached(newBridgeChild.GetJsSessionValue());
                performAfterBuild(newBridgeChild);
            });
        }

        public void RegisterInSession(object newCSharpObject, Action<IJsCsGlue> performAfterBuild) 
        {
            _JsUpdateHelper.CheckUiContext();

            var value = _JsObjectBuilder.Map(newCSharpObject);
            if (value == null)
                return;

            var updater = GetUnrootedEntitiesUpdater(value, performAfterBuild);
            _JsUpdateHelper.UpdateOnUiContext(updater, _ListenerUpdater.Off);
            if (!updater.HasUpdatesOnJavascriptContext)
                return;

            _JsUpdateHelper.DispatchInJavascriptContext(() =>
            {
                _JsUpdateHelper.UpdateOnJavascriptContext(updater, value);
            });
        } 

        public IJsCsGlue GetCachedOrCreateBasic(IJavascriptObject javascriptObject, Type targetType)
        {
            if (javascriptObject == null)
                return null;

            if (Context.WebView.Converter.GetSimpleValue(javascriptObject, out var targetValue, targetType)) {
                return new JsBasicObject(javascriptObject, targetValue);
            }

            if (targetType?.IsEnum == true)
            {
                var intValue = javascriptObject.GetValue("intValue")?.GetIntValue();
                if (!intValue.HasValue)
                    return null;

                targetValue = Enum.ToObject(targetType, intValue.Value);
                return new JsEnum(javascriptObject, (Enum)targetValue);
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
