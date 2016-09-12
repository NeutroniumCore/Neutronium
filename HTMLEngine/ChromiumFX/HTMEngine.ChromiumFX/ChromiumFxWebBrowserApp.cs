using Chromium;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx 
{
    public abstract class ChromiumFxWebBrowserApp : HTMLApp
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
