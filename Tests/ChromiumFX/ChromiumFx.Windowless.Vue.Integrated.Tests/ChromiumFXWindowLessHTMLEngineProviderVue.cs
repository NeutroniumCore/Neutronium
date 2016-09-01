using MVVM.HTML.Core;
using Tests.ChromiumFX.Infra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;
using VueUiFramework.Test.IntegratedInfra;

namespace ChromiumFx.Windowless.Vue.Integrated.Tests
{
    public class ChromiumFXWindowLessHTMLEngineProviderVue : IWindowLessHTMLEngineProvider 
    {
        private readonly ChromiumFXWindowLessHTMLEngineProvider _WindowBuilder = new ChromiumFXWindowLessHTMLEngineProvider(VueTestContext.HtmlProvider);

        private static FrameworkTestContext VueTestContext { get; } = VueFrameworkTestContext.GetVueFrameworkTestContext();
        public FrameworkTestContext FrameworkTestContext { get; } = VueTestContext;
        public IBasicWindowLessHTMLEngineProvider WindowBuilder => _WindowBuilder;

        public IWebSessionLogger Logger 
        {
            get { return _WindowBuilder.Logger; }
            set { _WindowBuilder.Logger = value; }
        }
    }
}
