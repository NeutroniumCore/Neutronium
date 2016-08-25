using ChromiumFX.TestInfra;
using UIFrameworkTesterHelper;
using VueUiFramework.Test.IntegratedInfra;

namespace ChromiumFX.Windowless.Integrated.Tests
{
    public class ChromiumFXWindowLessHTMLEngineProviderVue : ChromiumFXWindowLessHTMLEngineProvider
    {
        protected override FrameworkTestContext FrameworkTestContext => VueFrameworkTestContext.GetVueFrameworkTestContext();
    }
}
