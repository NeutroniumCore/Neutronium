using Neutronium.Core;
using Neutronium.WebBrowserEngine.CefGlue.CefSession;
using Neutronium.WPF;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue
{
    public class CefGlueWPFWebWindowFactory : IWPFWebWindowFactory
    {
        private readonly ICefCoreSession _ICefCoreSession;

        public string EngineName => "Chromium 51";
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
