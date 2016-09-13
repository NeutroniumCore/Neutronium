using CefGlue.TestInfra;
using KnockoutFramework.Test.IntegratedInfra;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;
using Tests.Infra.WebBrowserEngineTesterHelper.Context;

namespace Ko.Binding.ChromiumFx.Tests.Infra
{
    public class CfxKoContext : WindowLessHTMLEngineProvider
    {
        private static FrameworkTestContext KoTestContext { get; } = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();

        public override FrameworkTestContext FrameworkTestContext { get; } = KoTestContext;

        protected override IBasicWindowLessHTMLEngineProvider GetBasicWindowLessHTMLEngineProvider() => new CefGlueWindowlessSharedJavascriptEngineFactory(KoTestContext.HtmlProvider);
    }
}
