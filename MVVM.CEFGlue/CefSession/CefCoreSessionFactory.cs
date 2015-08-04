using CefGlue.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefSession
{
    public class CefCoreSessionFactory
    {
        private string[] _Args;
        private CefSettings _CefSettings;
        private IUIDispatcher _IUIDispatcher;

        public CefCoreSessionFactory(IUIDispatcher iIUIDispatcher, CefSettings iCefSettings = null, string[] args = null)
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

        //public CefSettings Settings 
        //{ 
        //    get { return _CefSettings; } 
        //}

        public ICefCoreSession GetSession()
        {
            return new CefCoreSession(_IUIDispatcher,_CefSettings, new MVVMCefApp(), _Args);
        }
    }
}
