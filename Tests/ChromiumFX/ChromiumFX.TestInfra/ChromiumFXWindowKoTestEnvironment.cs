using HTMEngine.ChromiumFX;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using KnockoutUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace ChromiumFX.TestInfra 
{
    public class ChromiumFXWindowKoTestEnvironment : WindowTestEnvironment 
    {
        public override IWPFWebWindowFactory GetWPFWebWindowFactory() 
        {
            return new ChromiumFXWPFWebWindowFactory();
        }

        public override IJavascriptUIFrameworkManager FrameworkManager 
        {
            get { return new KnockoutUiFrameworkManager(); }
        }
    }
}
