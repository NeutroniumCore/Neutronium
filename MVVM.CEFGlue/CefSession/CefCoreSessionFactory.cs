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

        public CefCoreSessionFactory(CefSettings iCefSettings=null, string[] args=null )
        {
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

            //var cefApp = new MVVMCefApp();
        }

        public CefSettings Settings 
        { 
            get { return _CefSettings; } 
        }

        public CefCoreSession GetSession()
        {
            return new CefCoreSession(_CefSettings, new MVVMCefApp(),_Args);
        }
    }
}
