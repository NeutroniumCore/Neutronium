using HTMLEngine.CefGlue;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using IntegratedTest.JavascriptUIFramework;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHtml;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace CefGlue.TestInfra 
{
    public class CefWindowTestEnvironment : WindowTestEnvironment 
    {
        public override IWPFWebWindowFactory GetWPFWebWindowFactory() 
        {
           return new CefGlueWPFWebWindowFactory();
        }

        public override IJavascriptUIFrameworkManager FrameworkManager => new KnockoutUiFrameworkManager();
        public override ITestHtmlProvider HtmlProvider => new KnockoutUiHtmlProvider();
    }
}
