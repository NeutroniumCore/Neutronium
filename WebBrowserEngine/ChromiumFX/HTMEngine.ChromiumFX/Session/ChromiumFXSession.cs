using System;
using Chromium;
using Chromium.Event;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;
using Neutronium.Core.Infra;
using System.Threading;
using Neutronium.WebBrowserEngine.ChromiumFx.WPF;
using Neutronium.Core;

namespace Neutronium.WebBrowserEngine.ChromiumFx.Session
{
    internal class ChromiumFxSession : IDisposable
    {
        private static ChromiumFxSession _Session = null;
        private readonly Action<CfxSettings> _SettingsBuilder;
        private readonly Action<CfxOnBeforeCommandLineProcessingEventArgs> _CommandLineHandler;
        private readonly string _CurrentDirectory;
        private readonly PackUriSchemeHandlerFactory _PackUriSchemeHandlerFactory;

        private ChromiumFxSession(Action<CfxSettings> settingsBuilder, Action<CfxOnBeforeCommandLineProcessingEventArgs> commadLineHandler, IWebSessionLogger webSessionLogger) 
        {
            _CurrentDirectory = this.GetType().Assembly.GetPath();
            _SettingsBuilder = settingsBuilder;
            _CommandLineHandler = commadLineHandler;
            CfxRuntime.LibCefDirPath = GetPath($@"{CefRepo}\Release");

            ChromiumWebBrowser.OnBeforeCfxInitialize += ChromiumWebBrowser_OnBeforeCfxInitialize;
            ChromiumWebBrowser.OnBeforeCommandLineProcessing += ChromiumWebBrowser_OnBeforeCommandLineProcessing;
            ChromiumWebBrowser.Initialize(true);

            _PackUriSchemeHandlerFactory = new PackUriSchemeHandlerFactory(webSessionLogger);
            //need this to make request interception work
            CfxRuntime.RegisterSchemeHandlerFactory("pack", null, _PackUriSchemeHandlerFactory);
        }   

        private static string CefRepo => (IntPtr.Size == 8) ? "cef64" : "cef";
        private string GetPath(string relativePath) 
        {
            return System.IO.Path.Combine(_CurrentDirectory, relativePath);
        }

        private void ChromiumWebBrowser_OnBeforeCommandLineProcessing(CfxOnBeforeCommandLineProcessingEventArgs e)
        {
            _CommandLineHandler?.Invoke(e);
        }

        private void ChromiumWebBrowser_OnBeforeCfxInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            var settings = e.Settings;

            _SettingsBuilder?.Invoke(settings);
         
            settings.LocalesDirPath = GetPath($@"{CefRepo}\Resources\locales");
            settings.Locale = Thread.CurrentThread.CurrentCulture.ToString();
            settings.ResourcesDirPath = GetPath($@"{CefRepo}\Resources");
            settings.BrowserSubprocessPath = GetPath("ChromiumFXRenderProcess.exe");
            settings.MultiThreadedMessageLoop = true;
            settings.NoSandbox = true;
        }

        internal static ChromiumFxSession GetSession(IWebSessionLogger logger, Action<CfxSettings> settingsBuilder, Action<CfxOnBeforeCommandLineProcessingEventArgs> commadLineHandler)
        {
            if (_Session != null)
                return _Session;

            _Session = new ChromiumFxSession(settingsBuilder, commadLineHandler, logger);
            return _Session;
        }

        public void Dispose() 
        {
            CfxRuntime.Shutdown();
        }
    }
}
