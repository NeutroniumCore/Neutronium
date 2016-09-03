using KnockoutUIFramework.Test.IntegratedInfra;
using Tests.Awesomium.Infra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace Ko.Binding.Awesomium.Tests.Infra
{
    public class AwesomiumKoContext : WindowLessHTMLEngineProvider
    {
        private static FrameworkTestContext KoTestContext { get; } = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();

        public override FrameworkTestContext FrameworkTestContext { get; } = KoTestContext;

        protected override IBasicWindowLessHTMLEngineProvider GetBasicWindowLessHTMLEngineProvider() => new AwesomiumEngineProvider(KoTestContext.HtmlProvider);
    }
}
