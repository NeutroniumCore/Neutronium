using System;
using Neutronium.Core.Exceptions;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.CefSession
{
    public class CefCoreSession : ICefCoreSession, IDisposable
    {
        private readonly CefSettings _CefSettings;
        private readonly string[] _Args;
        private readonly NeutroniumCefApp _CefApp;

        public CefCoreSession(CefSettings iCefSettings, NeutroniumCefApp iCefApp, string[] iArgs)
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

        public NeutroniumCefApp CefApp 
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
