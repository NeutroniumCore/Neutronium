using HTMLEngine.CefGlue.CefSession;
using HTML_WPF.Component;
using MVVM.HTML.Core;
using Xilium.CefGlue;

namespace HTMLEngine.CefGlue
{
    public class CefGlueWPFWebWindowFactory : IWPFWebWindowFactory
    {
        private readonly ICefCoreSession _ICefCoreSession;
        public CefGlueWPFWebWindowFactory( CefSettings iCefSettings = null)
        {
            _ICefCoreSession = CefCoreSessionSingleton.GetAndInitIfNeeded(iCefSettings);
        }

        public string EngineName
        {
            get { return "Chromium 41"; }
        }

        public string Name
        {
            get { return "Cef.Glue"; }
        }

        public IWPFWebWindow Create()
        {
            return new CefGlueWPFWebWindow(_ICefCoreSession.CefApp);
        }

        public IWebSessionWatcher WebSessionWatcher { get; set; }

        public void Dispose()
        {
            _ICefCoreSession.Dispose();
        }
    }
}
