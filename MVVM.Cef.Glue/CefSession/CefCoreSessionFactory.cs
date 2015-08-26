using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

using MVVM.HTML.Core.Window;

namespace MVVM.Cef.Glue.CefSession
{
    public class CefCoreSessionFactory
    {
        private string[] _Args;
        private CefSettings _CefSettings;
        private IDispatcher _IUIDispatcher;

        public CefCoreSessionFactory(IDispatcher iIUIDispatcher, CefSettings iCefSettings = null, string[] args = null)
        {
            _IUIDispatcher = iIUIDispatcher;
            _Args = args ?? new string[]{};

            _CefSettings = iCefSettings ?? new CefSettings
            {
                // BrowserSubprocessPath = browserSubprocessPath,
                SingleProcess = true,
                WindowlessRenderingEnabled = true,
                MultiThreadedMessageLoop = true,
                LogSeverity = CefLogSeverity.Disable,
                //LogFile = "cef.log",
                RemoteDebuggingPort = 8080
            };
        }

        public ICefCoreSession GetSession()
        {
            return new CefCoreSession(_IUIDispatcher,_CefSettings, new MVVMCefApp(), _Args);
        }
    }
}
