using CefGlue.TestInfra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;
using VueUiFramework.Test.IntegratedInfra;

namespace CefGlue.Windowless.Integrated.Vue.Tests
{
    public class CefGlueWindowlessSharedJavascriptEngineFactoryVue : WindowLessHTMLEngineProvider
    {
        private static FrameworkTestContext VueTestContext { get; } = VueFrameworkTestContext.GetVueFrameworkTestContext();
        public override FrameworkTestContext FrameworkTestContext { get; } = VueTestContext;
        protected override IBasicWindowLessHTMLEngineProvider GetBasicWindowLessHTMLEngineProvider() => new CefGlueWindowlessSharedJavascriptEngineFactory(VueTestContext.HtmlProvider);
    }
}
