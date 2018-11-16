using System;
using Chromium;
using Chromium.Event;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx
{
    public abstract class ChromiumFxWebBrowserApp : HTMLApp
    {
        protected virtual bool DisableGpu => true;

        protected override IWPFWebWindowFactory GetWindowFactory() =>
            new ChromiumFXWPFWebWindowFactory(UpdateChromiumSettings, Updater());

        protected virtual void UpdateChromiumSettings(CfxSettings settings) 
        {         
        }

        private Action<CfxOnBeforeCommandLineProcessingEventArgs> Updater() 
        {
            return DisableGpu ? PrivateUpdateLineCommandArg: default(Action<CfxOnBeforeCommandLineProcessingEventArgs>);
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
