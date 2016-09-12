using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.CefSession
{
    public class CefCoreSessionFactory
    {
        private readonly string[] _Args;
        private readonly CefSettings _CefSettings;

        public CefCoreSessionFactory(CefSettings iCefSettings = null, string[] args = null)
        {
            _Args = args ?? new string[]{};

            _CefSettings = iCefSettings ?? new CefSettings
            {
                SingleProcess = true,
                WindowlessRenderingEnabled = true,
                MultiThreadedMessageLoop = true,
                LogSeverity = CefLogSeverity.Disable,
                RemoteDebuggingPort = 8080
            };
        }

        public ICefCoreSession GetSession()
        {
            return new CefCoreSession(_CefSettings, new NeutroniumCefApp(), _Args);
        }
    }
}
