using HTML_WPF.Component;
using IntegratedTest.WPF.Infra;
using KnockoutUIFramework;
using MVVM.Cef.Glue;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace CefGlue.TestInfra 
{
    public class CefWindowTestEnvironment : WindowTestImprovedEnvironment 
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
