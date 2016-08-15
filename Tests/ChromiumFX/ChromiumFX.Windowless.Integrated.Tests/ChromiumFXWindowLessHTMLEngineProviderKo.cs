using ChromiumFX.TestInfra;
using IntegratedTest.JavascriptUIFramework;
using KnockoutUIFramework.Test.IntegratedInfra;

namespace ChromiumFX.Windowless.Integrated.Tests
{
    public class ChromiumFXWindowLessHTMLEngineProviderKo : ChromiumFXWindowLessHTMLEngineProvider
    {
        protected override FrameworkTestContext FrameworkTestContext => KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();
    }
}
