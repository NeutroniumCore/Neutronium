using HTMEngine.ChromiumFX;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using IntegratedTest.JavascriptUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;
using VueUiFramework;
using VueUiFramework.Test.TestHtml;

namespace ChromiumFX.TestInfra 
{
    public class ChromiumFXWindowVueTestEnvironment : WindowTestEnvironment 
    {
        public override IWPFWebWindowFactory GetWPFWebWindowFactory() 
        {
            return new ChromiumFXWPFWebWindowFactory();
        }

        public override IJavascriptUIFrameworkManager FrameworkManager => new VueSessionInjector();
        public override ITestHtmlProvider HtmlProvider => new VueUiHtmlProvider();
    }
}
