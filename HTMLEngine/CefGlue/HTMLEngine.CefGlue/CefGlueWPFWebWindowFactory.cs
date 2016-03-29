using HTML_WPF.Component;
using MVVM.Cef.Glue.CefSession;
using System;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue
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

        public Nullable<int> GetRemoteDebuggingPort()
        {
            return _ICefCoreSession.CefSettings.RemoteDebuggingPort;
        }

        public void Dispose()
        {
            _ICefCoreSession.Dispose();
        }
    }
}
