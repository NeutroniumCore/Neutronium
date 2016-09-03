using HTMLEngine.Awesomium;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHtml;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace ko.Navigation.Awesomium.Tests.Infra
{
    public class AwesomiumKoContext : WindowContextProvider
    {
        public override WindowTestContext GetWindowTestContext() => new WindowTestContext
        {
            WPFWebWindowFactory = () => new AwesomiumWPFWebWindowFactory(),
            FrameworkManager = new KnockoutUiFrameworkManager(),
            HtmlProvider = new KnockoutUiHtmlProvider()
        };
    }

}
