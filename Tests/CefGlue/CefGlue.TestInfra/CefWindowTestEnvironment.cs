using HTMLEngine.CefGlue;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using KnockoutUIFramework;
using MVVM.Cef.Glue;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace CefGlue.TestInfra 
{
    public class CefWindowTestEnvironment : WindowTestEnvironment 
    {
        public override IWPFWebWindowFactory GetWPFWebWindowFactory() 
        {
           return new CefGlueWPFWebWindowFactory();
        }

        public override IJavascriptUIFrameworkManager FrameworkManager 
        {
            get { return new KnockoutUiFrameworkManager();}
        }
    }
}
