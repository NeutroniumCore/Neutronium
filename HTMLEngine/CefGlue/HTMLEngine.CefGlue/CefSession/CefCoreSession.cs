using System;
using Xilium.CefGlue;

using MVVM.HTML.Core.Exceptions;

namespace MVVM.Cef.Glue.CefSession
{
    public class CefCoreSession : ICefCoreSession, IDisposable
    {
        private readonly CefSettings _CefSettings;
        private readonly string[] _Args;
        private readonly MVVMCefApp _CefApp;

        public CefCoreSession(CefSettings iCefSettings, MVVMCefApp iCefApp, string[] iArgs)
        {
            _CefApp = iCefApp;
            _Args = iArgs;
            _CefSettings = iCefSettings;
            var mainArgs = new CefMainArgs(_Args);

            CefRuntime.Load();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, _CefApp, IntPtr.Zero);
            if (exitCode != -1)
                throw ExceptionHelper.Get(string.Format("Unable to execute cef process: {0}", exitCode));

            CefRuntime.Initialize(mainArgs, _CefSettings, _CefApp, IntPtr.Zero);
        }

        public MVVMCefApp CefApp 
        { 
            get { return _CefApp; } 
        }

        public CefSettings CefSettings
        {
            get { return _CefSettings; }
        }

        public void Dispose()
        {
            CefRuntime.Shutdown();
        }
    }
}
