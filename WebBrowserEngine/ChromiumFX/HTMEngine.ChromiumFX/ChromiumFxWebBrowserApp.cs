using Chromium;
using Chromium.Event;
using Neutronium.Core;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx
{
    /// <summary>
    /// Wpf application with Neutronium set-up based on ChromiumFx
    /// </summary>
    public abstract class ChromiumFxWebBrowserApp : HTMLApp
    {
        /// <summary>
        /// Set to true to disable web security
        /// </summary>
        protected virtual bool DisableWebSecurity => false;

        /// <summary>
        /// Set to true to allow Gpu
        /// </summary>
        protected virtual bool AllowGpu => false;

        /// <summary>
        /// Override to alter chromium application settings
        /// </summary>
        /// <param name="settings"></param>
        protected virtual void UpdateChromiumSettings(CfxSettings settings) { }

        /// <summary>
        /// Override to alter chromium browser settings
        /// </summary>
        /// <param name="browserSettings"></param>
        protected virtual void UpdateChromiumBrowserSettings(CfxBrowserSettings browserSettings) { }

        /// <summary>
        /// Override to alter CEF command line
        /// </summary>
        /// <param name="beforeLineCommand"></param>
        protected virtual void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand) { }

        protected override IWPFWebWindowFactory GetWindowFactory(IWebSessionLogger logger) => new ChromiumFXWPFWebWindowFactory(logger, UpdateChromiumSettings, UpdateChromiumBrowserSettings, PrivateUpdateLineCommandArg);

        private void PrivateUpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
            var commandLine = beforeLineCommand.CommandLine;
            if (!AllowGpu)
                commandLine.AppendSwitch("disable-gpu");
            if (DisableWebSecurity)
                commandLine.AppendSwitch("disable-web-security");
            UpdateLineCommandArg(beforeLineCommand);
        }
    }
}
