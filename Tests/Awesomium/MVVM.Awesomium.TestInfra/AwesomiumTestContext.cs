using KnockoutUIFramework.Test.IntegratedInfra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace Tests.Awesomium.Infra 
{
    public class AwesomiumTestContext : WindowLessHTMLEngineProvider
    {
        private static FrameworkTestContext KoTestContext { get; } = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();

        public override FrameworkTestContext FrameworkTestContext { get; } = KoTestContext;

        protected override IBasicWindowLessHTMLEngineProvider GetBasicWindowLessHTMLEngineProvider() => new AwesomiumEngineProvider(KoTestContext.HtmlProvider);
    }
}
