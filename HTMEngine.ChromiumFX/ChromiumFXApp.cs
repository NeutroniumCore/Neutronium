using Chromium;
using HTML_WPF.Component;

namespace HTMEngine.ChromiumFX 
{
    public class ChromiumFXApp : HTMLApp
    {
        protected override IWPFWebWindowFactory GetWindowFactory() 
        {
            return new ChromiumFXWPFWebWindowFactory(UpdateChromiumSettings);
        }

        protected virtual void UpdateChromiumSettings(CfxSettings settings) 
        {         
        }
    }
}
