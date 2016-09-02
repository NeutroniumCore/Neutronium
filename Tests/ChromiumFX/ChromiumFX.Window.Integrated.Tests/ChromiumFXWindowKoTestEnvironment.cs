using HTMEngine.ChromiumFX;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHtml;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace ChromiumFX.Window.Integrated.Tests
{
    public class ChromiumFXWindowKoTestEnvironment : WindowContextProvider
    {
        public override WindowTestContext GetWindowTestContext() => new WindowTestContext
        {
            WPFWebWindowFactory = () => new ChromiumFXWPFWebWindowFactory(),
            FrameworkManager = new KnockoutUiFrameworkManager(),
            HtmlProvider = new KnockoutUiHtmlProvider()
        };
    }
}
