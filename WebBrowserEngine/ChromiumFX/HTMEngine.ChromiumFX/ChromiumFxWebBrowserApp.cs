using Chromium;
using Chromium.Event;
using Neutronium.Core;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx
{
    public abstract class ChromiumFxWebBrowserApp : HTMLApp
    {
        protected virtual bool DisableWebSecurity => true;

        protected override IWPFWebWindowFactory GetWindowFactory(IWebSessionLogger logger) =>
            new ChromiumFXWPFWebWindowFactory(logger, UpdateChromiumSettings, PrivateUpdateLineCommandArg);

        protected virtual void UpdateChromiumSettings(CfxSettings settings) 
        {         
        }

        private void PrivateUpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
            var commandLine = beforeLineCommand.CommandLine;
            commandLine.AppendSwitch("disable-gpu");
            // Needed to avoid crash when using devtools application tab with custom schema
            commandLine.AppendSwitch("disable-kill-after-bad-ipc");

            if (DisableWebSecurity)
                commandLine.AppendSwitch("disable-web-security");
            UpdateLineCommandArg(beforeLineCommand);
        }

        protected virtual void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
        }
    }
}
