using HTML_WPF.Component;
using IntegratedTest.WPF.Infra;
using KnockoutUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.Awesomium.TestInfra 
{
    public class AwesomiumWindowTestEnvironment : WindowTestImprovedEnvironment 
    {
        public AwesomiumWindowTestEnvironment() 
        {    
        }
        public override IWPFWebWindowFactory GetWPFWebWindowFactory()
        {
            return new AwesomiumWPFWebWindowFactory();
        }

        public override IJavascriptUIFrameworkManager FrameworkManager 
        {
            get { return new KnockoutUiFrameworkManager();}
        }
    }
}
