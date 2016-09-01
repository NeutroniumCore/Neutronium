using HTMLEngine.Awesomium;
using HTML_WPF.Component;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHtml;
using MVVM.HTML.Core.JavascriptUIFramework;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace Tests.Awesomium.Infra 
{
    public class AwesomiumWindowTestEnvironment : WindowTestEnvironment 
    {
        public override IWPFWebWindowFactory GetWPFWebWindowFactory()
        {
            return new AwesomiumWPFWebWindowFactory();
        }

        public override IJavascriptUIFrameworkManager FrameworkManager => new KnockoutUiFrameworkManager();
        public override ITestHtmlProvider HtmlProvider => new KnockoutUiHtmlProvider();
    }
}
