using HTMLEngine.CefGlue;
using KnockoutFramework.Test.TestHtml;
using Neutronium.JavascriptFramework.Knockout;
using Neutronium.WebBrowserEngine.CefGlue;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace Ko.Navigation.CefGlue.Tests.Infra
{
    public class CefGlueKoContext : WindowContextProvider
    {
        public override WindowTestContext GetWindowTestContext() => new WindowTestContext
        {
            WPFWebWindowFactory = () => new CefGlueWPFWebWindowFactory(),
            FrameworkManager = new KnockoutFrameworkManager(),
            HtmlProvider = new KnockoutHtmlProvider()
        };
    }

}
