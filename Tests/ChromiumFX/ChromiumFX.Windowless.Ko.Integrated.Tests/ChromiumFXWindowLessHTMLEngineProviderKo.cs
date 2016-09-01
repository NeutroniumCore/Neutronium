using KnockoutUIFramework.Test.IntegratedInfra;
using Tests.ChromiumFX.Infra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace ChromiumFX.Windowless.Ko.Integrated.Tests
{
    public class ChromiumFXWindowLessHTMLEngineProviderKo : IWindowLessHTMLEngineProvider 
    {
        private static FrameworkTestContext KoTestContext { get; } = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();

        public FrameworkTestContext FrameworkTestContext { get; } = KoTestContext;

        public IBasicWindowLessHTMLEngineProvider WindowBuilder { get; } = new ChromiumFXWindowLessHTMLEngineProvider(KoTestContext.HtmlProvider);
    }
}
