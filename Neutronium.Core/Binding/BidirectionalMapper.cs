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
        private readonly FullListenerRegister _ListenerRegister;
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
            var jsUpdateHelper = new JsUpdateHelper(this, Context, () => _BuilderStrategy, _SessionCache);
            _ListenerRegister = ListenToCSharp ? new FullListenerRegister(jsUpdateHelper) : null;
            glueFactory = glueFactory ?? GlueFactoryFactory.GetFactory(Context, _SessionCache, this, _ListenerRegister?.On);
            _JsObjectBuilder = new CSharpToJavascriptConverter(_SessionCache, glueFactory, _Logger);
            jsUpdateHelper.JsObjectBuilder =  _JsObjectBuilder;
            _JsUpdateHelper = jsUpdateHelper;
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
            _JsUpdateHelper.CheckUiContext();

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

            _OnJavascriptSessionReady?.Invoke(this, EventArgs.Empty);
        }

        private void RegisterJavascriptHelper()
        {
            var resource = new ResourceReader("scripts", this);
            var infa = resource.Load("Infra.js")
                .Replace(NeutroniumConstants.ReadOnlyFlagTemplate, NeutroniumConstants.ReadOnlyFlag)
                .Replace("{{ReadOnly}}", ((int)ObjectObservability.ReadOnly).ToString());

            Context.WebView.ExecuteJavaScript(infa);
        }

        private Task RunInJavascriptContext(Func<Task> run)
        {
            return Context.WebView.Evaluate(run);
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

        Task<IJavascriptObject> ISessionMapper.InjectInHtmlSession(IJsCsGlue root) => InjectInHtmlSession(root);

        private event EventHandler<EventArgs> _OnJavascriptSessionReady;
        event EventHandler<EventArgs> ISessionMapper.OnJavascriptSessionReady
        {
            add { _OnJavascriptSessionReady += value; }
            remove { _OnJavascriptSessionReady -= value; }
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
                            _ListenerRegister.OnCSharpPropertyChanged(currentfather.CValue, propertyName);
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

        private BridgeUpdater GetUnrootedEntitiesUpdater(IJsCsGlue newbridgedchild, Action<IJsCsGlue> performAfterBuild)
        {
            _UnrootedEntities.Add(newbridgedchild.AddRef());
            return new BridgeUpdater(updater => 
            {
                updater.InjectDetached(newbridgedchild.GetJsSessionValue());
                performAfterBuild(newbridgedchild);
            });
        }

        public void RegisterInSession(object newCSharpObject, Action<IJsCsGlue> performAfterBuild) 
        {
            _JsUpdateHelper.CheckUiContext();

            var value = _JsObjectBuilder.Map(newCSharpObject);
            if (value == null)
                return;

            var updater = GetUnrootedEntitiesUpdater(value, performAfterBuild);
            _JsUpdateHelper.UpdateOnUiContext(updater, _ListenerRegister.Off);
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
