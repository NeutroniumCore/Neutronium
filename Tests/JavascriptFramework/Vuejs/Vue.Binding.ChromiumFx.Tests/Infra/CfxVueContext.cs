using Tests.ChromiumFX.Infra;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptFrameworkTesterHelper;
using Tests.Infra.WebBrowserEngineTesterHelper.Context;
using VueFramework.Test.IntegratedInfra;

namespace Vue.Binding.ChromiumFx.Tests.Infra
{
    public class CfxVueContext : WindowLessHTMLEngineProvider
    {
        private static FrameworkTestContext VueTestContext { get; } = VueFrameworkTestContext.GetVueFrameworkTestContext();

        public override FrameworkTestContext FrameworkTestContext { get; } = VueTestContext;

        protected override IBasicWindowLessHTMLEngineProvider GetBasicWindowLessHTMLEngineProvider() => new ChromiumFXWindowLessHTMLEngineProvider(VueTestContext.HtmlProvider);
    }
}
