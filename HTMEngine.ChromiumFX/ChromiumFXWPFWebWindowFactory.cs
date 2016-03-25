using System;
using Chromium;
using Chromium.WebBrowser;
using HTMEngine.ChromiumFX.EngineBinding;
using HTMEngine.ChromiumFX.Session;
using HTML_WPF.Component;

namespace HTMEngine.ChromiumFX
{
    public class ChromiumFXWPFWebWindowFactory : IWPFWebWindowFactory 
    {
        public string EngineName 
        {
            get { return "Chromium" + CfxRuntime.GetChromeVersion(); }
        }

        public string Name 
        {
            get { return "ChromiumFX"; }
        }

        private readonly ChromiumFXSession _Session;
        private CfxSettings _Settings;

        public ChromiumFXWPFWebWindowFactory()
        {
            _Session = ChromiumFXSession.GetSession((settings) => 
            {
                settings.LocalesDirPath = System.IO.Path.GetFullPath(@"cef\Resources\locales");
                settings.ResourcesDirPath = System.IO.Path.GetFullPath(@"cef\Resources");
                settings.BrowserSubprocessPath = System.IO.Path.GetFullPath("ChromiumFXRenderProcess.exe");
                settings.RemoteDebuggingPort = 9090;
                settings.MultiThreadedMessageLoop = true;
                settings.SingleProcess = false;
            });
        }

        public CfxBrowserSettings BrtowserSettings 
        {
            get { return ChromiumWebBrowser.DefaultBrowserSettings; }
        }

        public IWPFWebWindow Create()
        {
            return new ChromiumFXWPFWindow();
        }

        public int? GetRemoteDebuggingPort()
        {
            return _Settings.RemoteDebuggingPort;
        }

        public void Dispose()
        {
            _Session.Dispose();
        }
    }
}
