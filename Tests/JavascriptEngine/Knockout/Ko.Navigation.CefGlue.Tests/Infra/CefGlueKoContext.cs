using HTMLEngine.CefGlue;
using KnockoutFramework.Test.TestHtml;
using Neutronium.JavascriptFramework.Knockout;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace Ko.Navigation.CefGlue.Tests.Infra
{
    public class CefGlueKoContext : WindowContextProvider
    {
        public override WindowTestContext GetWindowTestContext() => new WindowTestContext
        {
            WPFWebWindowFactory = () => new CefGlueWPFWebWindowFactory(),
            FrameworkManager = new KnockoutUiFrameworkManager(),
            HtmlProvider = new KnockoutHtmlProvider()
        };
    }

}
