using Chromium;
using Chromium.Event;
using Neutronium.Core;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx
{
    public abstract class ChromiumFxWebBrowserApp : HTMLApp
    {
        protected virtual bool DisableWebSecurity => false;

        protected override IWPFWebWindowFactory GetWindowFactory(IWebSessionLogger logger) =>
            new ChromiumFXWPFWebWindowFactory(logger, UpdateChromiumSettings, PrivateUpdateLineCommandArg);

        protected virtual void UpdateChromiumSettings(CfxSettings settings) 
        {         
        }

        private void PrivateUpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
            var commandLine = beforeLineCommand.CommandLine;
            commandLine.AppendSwitch("disable-gpu");
            if (DisableWebSecurity)
                commandLine.AppendSwitch("disable-web-security");
            UpdateLineCommandArg(beforeLineCommand);
        }

        protected virtual void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
        }
    }
}
