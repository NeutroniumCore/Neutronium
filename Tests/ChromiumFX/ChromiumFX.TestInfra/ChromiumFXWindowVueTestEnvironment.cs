using HTMEngine.ChromiumFX;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using MVVM.HTML.Core.JavascriptUIFramework;
using VueUiFramework;

namespace ChromiumFX.TestInfra 
{
    public class ChromiumFXWindowVueTestEnvironment : WindowTestEnvironment 
    {
        public override IWPFWebWindowFactory GetWPFWebWindowFactory() 
        {
            return new ChromiumFXWPFWebWindowFactory();
        }

        public override IJavascriptUIFrameworkManager FrameworkManager 
        {
            get { return new VueSessionInjector(); }
        }
    }
}
