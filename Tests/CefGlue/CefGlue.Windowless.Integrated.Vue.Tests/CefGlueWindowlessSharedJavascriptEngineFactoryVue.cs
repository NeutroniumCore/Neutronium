using CefGlue.TestInfra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;
using VueUiFramework.Test.IntegratedInfra;

namespace CefGlue.Windowless.Integrated.Vue.Tests
{
    public class CefGlueWindowlessSharedJavascriptEngineFactoryVue : IWindowLessHTMLEngineProvider 
    {
        private static FrameworkTestContext VueTestContext { get; } = VueFrameworkTestContext.GetVueFrameworkTestContext();

        public FrameworkTestContext FrameworkTestContext { get; } = VueTestContext;

        public IBasicWindowLessHTMLEngineProvider WindowBuilder { get; } = new CefGlueWindowlessSharedJavascriptEngineFactory(VueTestContext.HtmlProvider);
    }
}
