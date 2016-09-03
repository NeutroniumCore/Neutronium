using CefGlue.TestInfra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;
using VueUiFramework.Test.IntegratedInfra;

namespace Vue.Binding.CefGlue.Tests.Infra
{
    public class CefGlueVueContext : WindowLessHTMLEngineProvider
    {
        private static FrameworkTestContext VueTestContext { get; } = VueFrameworkTestContext.GetVueFrameworkTestContext();

        public override FrameworkTestContext FrameworkTestContext { get; } = VueTestContext;

        protected override IBasicWindowLessHTMLEngineProvider GetBasicWindowLessHTMLEngineProvider() => new CefGlueWindowlessSharedJavascriptEngineFactory(VueTestContext.HtmlProvider);
    }
}
