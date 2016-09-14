using KnockoutFramework.Test.TestHtml;
using Neutronium.JavascriptFramework.Knockout;
using Neutronium.WebBrowserEngine.ChromiumFx;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace Ko.Navigation.ChromiumFx.Tests.Infra
{
    public class CfxKoContext : WindowContextProvider
    {
        public override WindowTestContext GetWindowTestContext() => new WindowTestContext
        {
            WPFWebWindowFactory = () => new ChromiumFXWPFWebWindowFactory(),
            FrameworkManager = new KnockoutFrameworkManager(),
            HtmlProvider = new KnockoutHtmlProvider()
        };
    }

}
