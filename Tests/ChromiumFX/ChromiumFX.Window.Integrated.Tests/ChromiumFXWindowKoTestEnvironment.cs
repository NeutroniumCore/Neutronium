using HTMEngine.ChromiumFX;
using HTML_WPF.Component;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHtml;
using MVVM.HTML.Core.JavascriptUIFramework;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace ChromiumFX.Window.Integrated.Tests
{
    public class ChromiumFXWindowKoTestEnvironment : WindowTestEnvironment 
    {
        public override IWPFWebWindowFactory GetWPFWebWindowFactory() 
        {
            return new ChromiumFXWPFWebWindowFactory();
        }

        public override IJavascriptUIFrameworkManager FrameworkManager => new KnockoutUiFrameworkManager();
        public override ITestHtmlProvider HtmlProvider => new KnockoutUiHtmlProvider();
    }
}
