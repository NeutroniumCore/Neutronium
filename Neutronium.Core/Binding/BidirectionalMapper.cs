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
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Binding.Updater;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding
{
    public class BidirectionalMapper : IDisposable, ISessionMapper, ICSharpUnrootedObjectManager
    {
        private readonly IWebSessionLogger _Logger;
        private readonly ICSharpToGlueMapper _JsObjectBuilder;
        private readonly IJavascriptObjectBuilderStrategyFactory _BuilderStrategyFactory;
        private readonly ICSharpChangesListener _CSharpListenerJavascriptUpdater;
        private readonly IJavascriptChangesListener _JavascriptChangesListener;
        private readonly IJavascriptToGlueMapper _JavascriptToGlueMapper;
        private readonly List<IJsCsGlue> _UnrootedEntities = new List<IJsCsGlue>();
        private readonly SessionCacher _SessionCache;
        private readonly IJsUpdateHelper _JsUpdateHelper;

        private IJavascriptObjectBuilderStrategy _BuilderStrategy;
        private IJavascriptSessionInjector _SessionInjector;

        public IJsCsGlue JsValueRoot { get; }
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
            Context = contextBuilder.GetMainContext();
            _SessionCache = sessionCacher ?? new SessionCacher();
            var jsUpdateHelper = new JsUpdateHelper(this, Context, () => _BuilderStrategy, _SessionCache);
            _JavascriptToGlueMapper = new JavascriptToGlueMapper(jsUpdateHelper, _SessionCache);

            _CSharpListenerJavascriptUpdater = ListenToCSharp ? new CSharpListenerJavascriptUpdater(jsUpdateHelper) : null;
            glueFactory = glueFactory ?? GlueFactoryFactory.GetFactory(Context, _SessionCache, this, _JavascriptToGlueMapper, _CSharpListenerJavascriptUpdater?.On);
            _JsObjectBuilder = new CSharpToGlueMapper(_SessionCache, glueFactory, _Logger);
            jsUpdateHelper.GlueMapper =  _JsObjectBuilder;
            _JsUpdateHelper = jsUpdateHelper;
            _RootObject = root;
            _JavascriptChangesListener = (Mode == JavascriptBindingMode.TwoWay) ? 
                new JavascriptListenerCSharpUpdater(_CSharpListenerJavascriptUpdater, _JsUpdateHelper, _JavascriptToGlueMapper) : null;

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
            Context.InitOnJsContext(_JavascriptChangesListener, debugMode);
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
            exiting.ApplyOnListenable(_CSharpListenerJavascriptUpdater.Off);
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

        public void RegisterInSession(object newCSharpObject, Action<IJsCsGlue> performAfterBuild) 
        {
            _JsUpdateHelper.CheckUiContext();

            var value = _JsObjectBuilder.Map(newCSharpObject);
            if (value == null)
                return;

            var updater = GetUnrootedEntitiesUpdater(value, performAfterBuild);
            _JsUpdateHelper.DispatchInJavascriptContext(() =>
            {
                _JsUpdateHelper.UpdateOnJavascriptContext(updater, value);
            });
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
    }
}
