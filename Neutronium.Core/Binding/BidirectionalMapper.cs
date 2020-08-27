using System;
using System.Threading.Tasks;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.JavascriptFrameworkMapper;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Binding.SessionManagement;
using Neutronium.Core.Binding.Updater;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding
{
    public class BidirectionalMapper : IDisposable, IBindingLifeCycle
    {
        private readonly ICSharpToGlueMapper _CSharpToGlueMapper;
        private readonly IJavascriptToGlueMapper _JavascriptToGlueMapper;

        private readonly ICSharpChangesListener _CSharpChangesListener;
        private readonly IJavascriptChangesListener _JavascriptChangesListener;
       
        private readonly IInternalSessionCache _SessionCache;
        private readonly ICSharpUnrootedObjectManager _CSharpUnrootedObjectManager;

        private readonly Lazy<IJavascriptObjectBuilderStrategy> _BuilderStrategy;
        private readonly Lazy<IJavascriptFrameworkMapper> _JavascriptFrameworkManager;

        private IJavascriptObjectBuilderStrategy BuilderStrategy => _BuilderStrategy.Value;
        private IJavascriptFrameworkMapper JavascriptFrameworkMapper => _JavascriptFrameworkManager.Value;

        public IJsCsGlue JsValueRoot { get; }
        public JavascriptBindingMode Mode { get; }
        public HtmlViewContext Context { get; }
        public bool ListenToCSharp => (Mode != JavascriptBindingMode.OneTime);
        public IWebSessionLogger Logger => Context.Logger;

        internal BidirectionalMapper(object root, HtmlViewEngine engine, JavascriptBindingMode mode, IJavascriptObjectBuilderStrategyFactory strategyFactory) :
            this(root, engine, null, strategyFactory, mode, null)
        {
        }

        internal BidirectionalMapper(object root, HtmlViewEngine contextBuilder, IGlueFactory glueFactory, IJavascriptObjectBuilderStrategyFactory strategyFactory, 
            JavascriptBindingMode mode, IInternalSessionCache sessionCacher)
        {
            Mode = mode;
            Context = contextBuilder.GetMainContext();
            _SessionCache = sessionCacher ?? new SessionCacher();

            strategyFactory = strategyFactory ?? new StandardStrategyFactory();
            _BuilderStrategy = new Lazy<IJavascriptObjectBuilderStrategy>(() =>
                strategyFactory.GetStrategy(Context.WebView, _SessionCache, Context.JavascriptFrameworkIsMappingObject)
            );
            _JavascriptFrameworkManager = new Lazy<IJavascriptFrameworkMapper>(() => Context.GetMapper(_SessionCache, BuilderStrategy));

            var jsUpdateHelper = new JsUpdateHelper(this, Context, _JavascriptFrameworkManager, _SessionCache);
            _JavascriptToGlueMapper = new JavascriptToGlueMapper(jsUpdateHelper, _SessionCache);

            _CSharpChangesListener = ListenToCSharp ? new CSharpListenerJavascriptUpdater(jsUpdateHelper) : null;
            glueFactory = glueFactory ?? GlueFactoryFactory.GetFactory(Context, _SessionCache, _JavascriptToGlueMapper, _CSharpChangesListener?.On);
            _CSharpToGlueMapper = new CSharpToGlueMapper(_SessionCache, glueFactory, Context.Logger);
            jsUpdateHelper.GlueMapper =  _CSharpToGlueMapper;
            
            _CSharpUnrootedObjectManager = new CSharpUnrootedObjectManager(jsUpdateHelper, _CSharpToGlueMapper);
            glueFactory.UnrootedObjectManager = _CSharpUnrootedObjectManager;
            _JavascriptChangesListener = (Mode == JavascriptBindingMode.TwoWay) ? 
                new JavascriptListenerCSharpUpdater(_CSharpChangesListener, jsUpdateHelper, _JavascriptToGlueMapper) : null;

            jsUpdateHelper.CheckUiContext();
            JsValueRoot = _CSharpToGlueMapper.Map(root);
            JsValueRoot.AddRef();
        }

        internal Task UpdateJavascriptObjects(bool debugMode)
        {
            return Context.WebView.Evaluate(() => InitOnJavascriptContext(debugMode));
        }

        private async Task InitOnJavascriptContext(bool debugMode)
        {
            RegisterJavascriptHelper();
            Context.InitOnJsContext(_JavascriptChangesListener, debugMode);
            BuilderStrategy.UpdateJavascriptValue(JsValueRoot);
            var rootVm = await JavascriptFrameworkMapper.UpdateJavascriptObject(JsValueRoot);
            await RegisterMain(rootVm);
        }

        private Task RegisterMain(IJavascriptObject res)
        {
            var registerTask = Context.JavascriptSessionInjector.RegisterMainViewModel(res);
            Context.UiDispatcher.Dispatch(JavascriptReady);
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

        public void Dispose()
        {
            Context.CheckUiContext();
            if (ListenToCSharp)
                OnAllJsGlues(UnlistenGlue);

            Context.Dispose();
            _CSharpUnrootedObjectManager.Dispose();
            Logger.Debug("BidirectionalMapper disposed");
            _BuilderStrategy.LazyDo(builder => builder.Dispose());
        }

        private void UnlistenGlue(IJsCsGlue exiting) => exiting.ApplyOnListenable(_CSharpChangesListener.Off);

        private void OnAllJsGlues(Action<IJsCsGlue> @do) => _SessionCache.AllElementsUiContext.ForEach(@do);

        private event EventHandler<EventArgs> _OnJavascriptSessionReady;
        event EventHandler<EventArgs> IBindingLifeCycle.OnJavascriptSessionReady
        {
            add => _OnJavascriptSessionReady += value;
            remove => _OnJavascriptSessionReady -= value;
        }
    }
}
