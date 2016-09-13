using HTMLEngine.Awesomium;
using KnockoutFramework.Test.TestHtml;
using Neutronium.JavascriptFramework.Knockout;
using Neutronium.WebBrowserEngine.Awesomium;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace ko.Navigation.Awesomium.Tests.Infra
{
    public class AwesomiumKoContext : WindowContextProvider
    {
        public override WindowTestContext GetWindowTestContext() => new WindowTestContext
        {
            WPFWebWindowFactory = () => new AwesomiumWPFWebWindowFactory(),
            FrameworkManager = new KnockoutFrameworkManager(),
            HtmlProvider = new KnockoutHtmlProvider()
        };
    }

}
