using HTML_WPF.Component;
using MVVM.Cef.Glue.CefSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue
{
    public class CefGlueWPFWebWindowFactory : IWPFWebWindowFactory
    {
        private ICefCoreSession _ICefCoreSession;
        public CefGlueWPFWebWindowFactory( CefSettings iCefSettings = null)
        {
            _ICefCoreSession = CefCoreSessionSingleton.GetAndInitIfNeeded(iCefSettings);
        }


        public string Name
        {
            get { return "Cef.Glue"; }
        }

        public IWPFWebWindow Create()
        {
            return new CefGlueWPFWebWindow();
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
