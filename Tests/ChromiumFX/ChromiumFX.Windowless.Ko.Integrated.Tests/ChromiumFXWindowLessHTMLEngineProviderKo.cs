using ChromiumFX.TestInfra;
using IntegratedTest.JavascriptUIFramework;
using KnockoutUIFramework.Test.IntegratedInfra;

namespace ChromiumFX.Windowless.Ko.Integrated.Tests
{
    public class ChromiumFXWindowLessHTMLEngineProviderKo : ChromiumFXWindowLessHTMLEngineProvider
    {
        protected override FrameworkTestContext FrameworkTestContext => KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();
    }
}
