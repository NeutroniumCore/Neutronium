using HTMLEngine.CefGlue;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHtml;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace CefGlue.Window.Integrated.Tests
{
    public class CefWindowTestEnvironmentKo : WindowContextProvider 
    {
        public override WindowTestContext GetWindowTestContext() => new WindowTestContext 
        {
            WPFWebWindowFactory = () => new CefGlueWPFWebWindowFactory(),
            FrameworkManager = new KnockoutUiFrameworkManager(),
            HtmlProvider = new KnockoutUiHtmlProvider()
        };
    }
}
