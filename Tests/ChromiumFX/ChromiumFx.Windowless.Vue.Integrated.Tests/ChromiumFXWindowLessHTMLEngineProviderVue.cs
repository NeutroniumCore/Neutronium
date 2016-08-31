using ChromiumFX.TestInfra;
using Tests.Infra.JavascriptEngineTesterHelper;
using VueUiFramework.Test.IntegratedInfra;

namespace ChromiumFX.Windowless.Integrated.Tests
{
    public class ChromiumFXWindowLessHTMLEngineProviderVue : ChromiumFXWindowLessHTMLEngineProvider
    {
        protected override FrameworkTestContext FrameworkTestContext => VueFrameworkTestContext.GetVueFrameworkTestContext();
    }
}
