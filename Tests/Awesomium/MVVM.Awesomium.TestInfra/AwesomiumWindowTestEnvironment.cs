using HTMLEngine.Awesomium;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using KnockoutUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.Awesomium.TestInfra 
{
    public class AwesomiumWindowTestEnvironment : WindowTestEnvironment 
    {
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
