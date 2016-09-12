using System;
using Chromium;
using Chromium.WebBrowser;
using HTMEngine.ChromiumFX.EngineBinding;
using HTMEngine.ChromiumFX.Session;
using Neutronium.Core;
using Neutronium.WPF;

namespace HTMEngine.ChromiumFX
{
    public class ChromiumFXWPFWebWindowFactory : IWPFWebWindowFactory 
    {
        private readonly ChromiumFXSession _Session;

        public CfxSettings Settings { get; private set; }
        public string EngineName => "Chromium " + CfxRuntime.GetChromeVersion();
        public string Name => "ChromiumFX";
        public CfxBrowserSettings BrtowserSettings => ChromiumWebBrowser.DefaultBrowserSettings;
        public IWebSessionLogger WebSessionLogger { get; set; }

        public ChromiumFXWPFWebWindowFactory(Action<CfxSettings> settingsUpdater=null)
        {
            _Session = ChromiumFXSession.GetSession((settings) => 
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
            return new ChromiumFXWPFWindow(WebSessionLogger);
        }

        public void Dispose()
        {
            _Session.Dispose();
        }
    }
}
