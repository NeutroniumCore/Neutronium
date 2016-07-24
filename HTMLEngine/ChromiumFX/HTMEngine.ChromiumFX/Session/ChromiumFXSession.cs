using System;
using Chromium;
using Chromium.Event;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;

namespace HTMEngine.ChromiumFX.Session
{
    internal class ChromiumFXSession : IDisposable
    {
        private static ChromiumFXSession _Session = null;
        private readonly Action<CfxSettings> _SettingsBuilder;

        private ChromiumFXSession(Action<CfxSettings> settingsBuilder) 
        {
            _SettingsBuilder = settingsBuilder;
            CfxRuntime.LibCefDirPath = @"cef\Release";

            ChromiumWebBrowser.OnBeforeCfxInitialize += ChromiumWebBrowser_OnBeforeCfxInitialize;
            ChromiumWebBrowser.OnBeforeCommandLineProcessing += ChromiumWebBrowser_OnBeforeCommandLineProcessing;
            ChromiumWebBrowser.Initialize();
        }

        private void ChromiumWebBrowser_OnBeforeCommandLineProcessing(CfxOnBeforeCommandLineProcessingEventArgs e)
        {
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

        internal static ChromiumFXSession GetSession(Action<CfxSettings> settingsBuilder)
        {
            if (_Session != null)
                return _Session;

            _Session = new ChromiumFXSession(settingsBuilder);
            return _Session;
        }

        public void Dispose() 
        {
            CfxRuntime.Shutdown();
        }
    }
}
