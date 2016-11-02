using Chromium;
using Chromium.Event;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx 
{
    public abstract class ChromiumFxWebBrowserApp : HTMLApp
    {
        protected override IWPFWebWindowFactory GetWindowFactory() 
        {
            return new ChromiumFXWPFWebWindowFactory(UpdateChromiumSettings, UpdateLineCommandArg);
        }

        protected virtual void UpdateChromiumSettings(CfxSettings settings) 
        {         
        }

        protected virtual void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
        }
    }
}
