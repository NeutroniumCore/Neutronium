using HTMEngine.ChromiumFX;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using IntegratedTest.JavascriptUIFramework;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHtml;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace ChromiumFX.TestInfra 
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
