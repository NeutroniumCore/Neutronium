using ChromiumFX.TestInfra;
using KnockoutUIFramework.Test.IntegratedInfra;
using UIFrameworkTesterHelper;

namespace ChromiumFX.Windowless.Ko.Integrated.Tests
{
    public class ChromiumFXWindowLessHTMLEngineProviderKo : ChromiumFXWindowLessHTMLEngineProvider
    {
        protected override FrameworkTestContext FrameworkTestContext => KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();
    }
}
