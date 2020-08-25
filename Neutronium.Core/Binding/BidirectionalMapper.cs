using System;
using System.Threading.Tasks;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.JavascriptFrameworkMapper;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Binding.SessionManagement;
using Neutronium.Core.Binding.Updater;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding
{
    public class BidirectionalMapper : IDisposable, IBindingLifeCicle
    {
        private readonly IWebSessionLogger _Logger;

        private readonly ICSharpToGlueMapper _CSharpToGlueMapper;
        private readonly IJavascriptToGlueMapper _JavascriptToGlueMapper;

        private readonly IJavascriptObjectBuilderStrategyFactory _BuilderStrategyFactory;

        private readonly ICSharpChangesListener _CSharpChangesListener;
        private readonly IJavascriptChangesListener _JavascriptChangesListener;
       
        private readonly SessionCacher _SessionCache;
        private readonly IJsUpdateHelper _JsUpdateHelper;

        private readonly ICSharpUnrootedObjectManager _CSharpUnrootedObjectManager;

        private IJavascriptObjectBuilderStrategy _BuilderStrategy;
        private IJavascriptSessionInjector _SessionInjector;
        private IJavascriptFrameworkMapper _JavascriptFrameworkManager;

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
            var jsUpdateHelper = new JsUpdateHelper(this, Context, () => _JavascriptFrameworkManager, _SessionCache);
            _JavascriptToGlueMapper = new JavascriptToGlueMapper(jsUpdateHelper, _SessionCache);

            _CSharpChangesListener = ListenToCSharp ? new CSharpListenerJavascriptUpdater(jsUpdateHelper) : null;
            glueFactory = glueFactory ?? GlueFactoryFactory.GetFactory(Context, _SessionCache, _JavascriptToGlueMapper, _CSharpChangesListener?.On);
            _CSharpToGlueMapper = new CSharpToGlueMapper(_SessionCache, glueFactory, _Logger);
            jsUpdateHelper.GlueMapper =  _CSharpToGlueMapper;
            _JsUpdateHelper = jsUpdateHelper;
            _RootObject = root;
            _CSharpUnrootedObjectManager = new CSharpUnrootedObjectManager(_JsUpdateHelper, _CSharpToGlueMapper);
            glueFactory.UnrootedObjectManager = _CSharpUnrootedObjectManager;
            _JavascriptChangesListener = (Mode == JavascriptBindingMode.TwoWay) ? 
                new JavascriptListenerCSharpUpdater(_CSharpChangesListener, _JsUpdateHelper, _JavascriptToGlueMapper) : null;

            _JsUpdateHelper.CheckUiContext();
            JsValueRoot = _CSharpToGlueMapper.Map(_RootObject);
            JsValueRoot.AddRef();
        }

        internal Task UpdateJavascriptObjects(bool debugMode)
        {
            return RunInJavascriptContext(() => InitOnJavascriptContext(debugMode));
        }

        private async Task InitOnJavascriptContext(bool debugMode)
        {
            RegisterJavascriptHelper();
            Context.InitOnJsContext(_JavascriptChangesListener, debugMode);
            _SessionInjector = Context.JavascriptSessionInjector;
            _BuilderStrategy = _BuilderStrategyFactory.GetStrategy(Context.WebView, _SessionCache, Context.JavascriptFrameworkIsMappingObject);
            _BuilderStrategy.UpdateJavascriptValue(JsValueRoot);

            _JavascriptFrameworkManager = Context.GetMapper(_SessionCache, _BuilderStrategy);
            var rootVm = await _JavascriptFrameworkManager.UpdateJavascriptObject(JsValueRoot);
            await RegisterMain(rootVm);
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
            _CSharpUnrootedObjectManager.Dispose();
            _Logger.Debug("BidirectionalMapper disposed");
            _BuilderStrategy?.Dispose();
        }

        private void UnlistenGlue(IJsCsGlue exiting) => exiting.ApplyOnListenable(_CSharpChangesListener.Off);

        private void OnAllJsGlues(Action<IJsCsGlue> @do) => _SessionCache.AllElementsUiContext.ForEach(@do);

        private event EventHandler<EventArgs> _OnJavascriptSessionReady;
        event EventHandler<EventArgs> IBindingLifeCicle.OnJavascriptSessionReady
        {
            add => _OnJavascriptSessionReady += value;
            remove => _OnJavascriptSessionReady -= value;
        }
    }
}
