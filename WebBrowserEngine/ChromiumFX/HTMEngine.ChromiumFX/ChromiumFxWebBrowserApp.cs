using Chromium;
using Chromium.Event;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx
{
    public abstract class ChromiumFxWebBrowserApp : HTMLApp
    {
        protected override IWPFWebWindowFactory GetWindowFactory() =>
            new ChromiumFXWPFWebWindowFactory(UpdateChromiumSettings, PrivateUpdateLineCommandArg);

        protected virtual void UpdateChromiumSettings(CfxSettings settings) 
        {         
        }

        private void PrivateUpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand) 
        {
            beforeLineCommand.CommandLine.AppendSwitch("disable-gpu");
            UpdateLineCommandArg(beforeLineCommand);
        }

        protected virtual void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
        }
    }
}
