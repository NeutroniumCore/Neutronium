using HTMLEngine.CefGlue.CefSession;
using Neutronium.Core;
using Neutronium.WPF;
using Xilium.CefGlue;

namespace HTMLEngine.CefGlue
{
    public class CefGlueWPFWebWindowFactory : IWPFWebWindowFactory
    {
        private readonly ICefCoreSession _ICefCoreSession;

        public string EngineName => "Chromium 41";
        public string Name => "Cef.Glue";
        public IWebSessionLogger WebSessionLogger { get; set; }

        public CefGlueWPFWebWindowFactory( CefSettings iCefSettings = null)
        {
            _ICefCoreSession = CefCoreSessionSingleton.GetAndInitIfNeeded(iCefSettings);
        }

        public IWPFWebWindow Create()
        {
            return new CefGlueWPFWebWindow(_ICefCoreSession.CefApp);
        }

        public void Dispose()
        {
            _ICefCoreSession.Dispose();
        }
    }
}
