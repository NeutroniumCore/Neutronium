using Chromium;
using Chromium.Event;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx
{
    public abstract class ChromiumFxWebBrowserApp : HTMLApp
    {
        protected virtual bool DisableWebSecurity => false;

        protected override IWPFWebWindowFactory GetWindowFactory() =>
            new ChromiumFXWPFWebWindowFactory(UpdateChromiumSettings, PrivateUpdateLineCommandArg);

        protected virtual void UpdateChromiumSettings(CfxSettings settings) 
        {         
        }

        private void PrivateUpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand) 
        {
            beforeLineCommand.CommandLine.AppendSwitch("disable-gpu");
            if (DisableWebSecurity)
                beforeLineCommand.CommandLine.AppendSwitch("disable-web-security");
            UpdateLineCommandArg(beforeLineCommand);
        }

        protected virtual void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
        }
    }
}
