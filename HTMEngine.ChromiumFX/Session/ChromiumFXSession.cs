using System;
using Chromium;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;

namespace HTMEngine.ChromiumFX.Session
{
    internal class ChromiumFXSession : IDisposable
    {
        private static ChromiumFXSession _Session = null;
        private readonly Action<CfxSettings> _SettingsBuilder;
        private CfxSettings _Settings;

        private ChromiumFXSession(Action<CfxSettings> settingsBuilder) 
        {
            _SettingsBuilder = settingsBuilder;
            CfxRuntime.LibCefDirPath = @"cef\Release";

            ChromiumWebBrowser.OnBeforeCfxInitialize += ChromiumWebBrowser_OnBeforeCfxInitialize;
            ChromiumWebBrowser.Initialize();
        }

        private void ChromiumWebBrowser_OnBeforeCfxInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            var settings = e.Settings;
         
            if (_SettingsBuilder != null) 
            {
                _SettingsBuilder(settings);
            }

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
