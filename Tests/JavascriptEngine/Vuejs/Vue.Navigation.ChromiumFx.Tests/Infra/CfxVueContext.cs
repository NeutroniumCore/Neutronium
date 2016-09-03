using HTMEngine.ChromiumFX;
using Tests.Infra.IntegratedContextTesterHelper.Window;
using VueUiFramework;
using VueUiFramework.Test.TestHtml;

namespace Vue.Navigation.ChromiumFx.Tests.Infra
{
    public class CfxVueContext : WindowContextProvider
    {
        public override WindowTestContext GetWindowTestContext() => new WindowTestContext
        {
            WPFWebWindowFactory = () => new ChromiumFXWPFWebWindowFactory(),
            FrameworkManager = new VueSessionInjector(),
            HtmlProvider = new VueUiHtmlProvider()
        };
    }
}
