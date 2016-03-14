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

        public ChromiumFXWPFWebWindowFactory()
        {
            _Session = ChromiumFXSession.GetSession();
        }

        public IWPFWebWindow Create()
        {
            return new ChromiumFXWPFWindow();
        }

        public int? GetRemoteDebuggingPort()
        {
            return null;
        }

        public void Dispose()
        {
            _Session.Dispose();
        }
    }
}
