using System;
using Chromium;
using Chromium.WebBrowser;
using Neutronium.Core;
using Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding;
using Neutronium.WebBrowserEngine.ChromiumFx.Session;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx
{
    public class ChromiumFXWPFWebWindowFactory : IWPFWebWindowFactory 
    {
        private readonly ChromiumFxSession _Session;

        public CfxSettings Settings { get; private set; }
        public string EngineName => "Chromium " + CfxRuntime.GetChromeVersion();
        public string Name => "ChromiumFX";
        public CfxBrowserSettings BrtowserSettings => ChromiumWebBrowser.DefaultBrowserSettings;
        public IWebSessionLogger WebSessionLogger { get; set; }

        public ChromiumFXWPFWebWindowFactory(Action<CfxSettings> settingsUpdater=null)
        {
            _Session = ChromiumFxSession.GetSession((settings) => 
            {
                settingsUpdater?.Invoke(settings);

                settings.LocalesDirPath = System.IO.Path.GetFullPath(@"cef\Resources\locales");
                settings.ResourcesDirPath = System.IO.Path.GetFullPath(@"cef\Resources");
                settings.BrowserSubprocessPath = System.IO.Path.GetFullPath("ChromiumFXRenderProcess.exe");
                settings.MultiThreadedMessageLoop = true;
                settings.SingleProcess = false;

                Settings = settings;
            });
        }

        public IWPFWebWindow Create()
        {
            return new ChromiumFxWpfWindow(WebSessionLogger);
        }

        public void Dispose()
        {
            _Session.Dispose();
        }
    }
}
