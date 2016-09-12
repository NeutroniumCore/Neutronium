using Chromium;
using Neutronium.WPF;

namespace HTMEngine.ChromiumFX 
{
    public abstract class ChromiumFXApp : HTMLApp
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
