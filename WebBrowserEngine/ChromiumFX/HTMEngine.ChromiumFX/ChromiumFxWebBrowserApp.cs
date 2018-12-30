using Chromium;
using Chromium.Event;
using Neutronium.Core;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx
{
    public abstract class ChromiumFxWebBrowserApp : HTMLApp
    {
        protected virtual bool DisableWebSecurity => false;

        protected virtual void UpdateChromiumSettings(CfxSettings settings) { }

        protected virtual void UpdateChromiumBrowserSettings(CfxBrowserSettings browserSettings) { }

        protected virtual void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand) { }

        protected override IWPFWebWindowFactory GetWindowFactory(IWebSessionLogger logger) => new ChromiumFXWPFWebWindowFactory(logger, UpdateChromiumSettings, UpdateChromiumBrowserSettings, PrivateUpdateLineCommandArg);

        private void PrivateUpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
            var commandLine = beforeLineCommand.CommandLine;
            commandLine.AppendSwitch("disable-gpu");
            if (DisableWebSecurity)
                commandLine.AppendSwitch("disable-web-security");
            UpdateLineCommandArg(beforeLineCommand);
        }
    }
}
