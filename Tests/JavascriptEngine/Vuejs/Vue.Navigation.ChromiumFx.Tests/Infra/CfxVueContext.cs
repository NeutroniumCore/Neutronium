using HTMEngine.ChromiumFX;
using Neutronium.JavascriptFramework.Vue;
using Tests.Infra.IntegratedContextTesterHelper.Window;
using VueFramework.Test.TestHtml;

namespace Vue.Navigation.ChromiumFx.Tests.Infra
{
    public class CfxVueContext : WindowContextProvider
    {
        public override WindowTestContext GetWindowTestContext() => new WindowTestContext
        {
            WPFWebWindowFactory = () => new ChromiumFXWPFWebWindowFactory(),
            FrameworkManager = new VueSessionInjector(),
            HtmlProvider = new VueHtmlProvider()
        };
    }
}
