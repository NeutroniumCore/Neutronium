using System;
using Chromium;
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
            get { return "ChromiumFX V.0.0.1"; }
        }

        private ChromiumFXSession _Session;
        private CfxSettings _Settings;

        public ChromiumFXWPFWebWindowFactory(Func<CfxSettings> settingsBuilder = null)
        {
            _Session = ChromiumFXSession.GetSession(settingsBuilder);
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
