using HTMEngine.ChromiumFX;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHtml;
using Neutronium.JavascriptFramework.Knockout;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace Ko.Navigation.ChromiumFx.Tests.Infra
{
    public class CfxKoContext : WindowContextProvider
    {
        public override WindowTestContext GetWindowTestContext() => new WindowTestContext
        {
            WPFWebWindowFactory = () => new ChromiumFXWPFWebWindowFactory(),
            FrameworkManager = new KnockoutUiFrameworkManager(),
            HtmlProvider = new KnockoutUiHtmlProvider()
        };
    }

}
