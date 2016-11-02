using System;
using Chromium;
using Chromium.Event;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;

namespace Neutronium.WebBrowserEngine.ChromiumFx.Session
{
    internal class ChromiumFxSession : IDisposable
    {
        private static ChromiumFxSession _Session = null;
        private readonly Action<CfxSettings> _SettingsBuilder;
        private readonly Action<CfxOnBeforeCommandLineProcessingEventArgs> _CommandLineHandler;

        private ChromiumFxSession(Action<CfxSettings> settingsBuilder, Action<CfxOnBeforeCommandLineProcessingEventArgs> commadLineHandler) 
        {
            _SettingsBuilder = settingsBuilder;
            _CommandLineHandler = commadLineHandler;
            CfxRuntime.LibCefDirPath = @"cef\Release";

            ChromiumWebBrowser.OnBeforeCfxInitialize += ChromiumWebBrowser_OnBeforeCfxInitialize;
            ChromiumWebBrowser.OnBeforeCommandLineProcessing += ChromiumWebBrowser_OnBeforeCommandLineProcessing;
            ChromiumWebBrowser.Initialize();
        }

        private void ChromiumWebBrowser_OnBeforeCommandLineProcessing(CfxOnBeforeCommandLineProcessingEventArgs e)
        {
            _CommandLineHandler?.Invoke(e);
        }

        private void ChromiumWebBrowser_OnBeforeCfxInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            var settings = e.Settings;

            _SettingsBuilder?.Invoke(settings);

            settings.NoSandbox = true;
            settings.LocalesDirPath = System.IO.Path.GetFullPath(@"cef\Resources\locales");
            settings.ResourcesDirPath = System.IO.Path.GetFullPath(@"cef\Resources");
            settings.BrowserSubprocessPath = System.IO.Path.GetFullPath("ChromiumFXRenderProcess.exe");
            settings.SingleProcess = false;
        }

        internal static ChromiumFxSession GetSession(Action<CfxSettings> settingsBuilder, Action<CfxOnBeforeCommandLineProcessingEventArgs> commadLineHandler)
        {
            if (_Session != null)
                return _Session;

            _Session = new ChromiumFxSession(settingsBuilder, commadLineHandler);
            return _Session;
        }

        public void Dispose() 
        {
            CfxRuntime.Shutdown();
        }
    }
}
