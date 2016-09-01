using KnockoutUIFramework.Test.IntegratedInfra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace Tests.Awesomium.Infra 
{
    public class AwesomiumTestContext : IWindowLessHTMLEngineProvider 
    {
        private static FrameworkTestContext KoTestContext { get; } = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();

        public FrameworkTestContext FrameworkTestContext { get; } = KoTestContext;

        public IBasicWindowLessHTMLEngineProvider WindowBuilder { get; } = new AwesomiumEngineProvider(KoTestContext.HtmlProvider);
    }
}
