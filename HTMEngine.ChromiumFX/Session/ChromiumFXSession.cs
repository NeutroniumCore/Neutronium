using System;
using Chromium;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;

namespace HTMEngine.ChromiumFX.Session
{
    internal class ChromiumFXSession : IDisposable
    {
        private static ChromiumFXSession _Session = null;
        private readonly Func<CfxSettings> _SettingsBuilder;
        private CfxSettings _Settings;

        private ChromiumFXSession(Func<CfxSettings> settingsBuilder) 
        {
            _SettingsBuilder = settingsBuilder;
            CfxRuntime.LibCefDirPath = @"cef\Release";

            ChromiumWebBrowser.OnBeforeCfxInitialize += ChromiumWebBrowser_OnBeforeCfxInitialize;
            ChromiumWebBrowser.Initialize();
        }

        private void ChromiumWebBrowser_OnBeforeCfxInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            var settings = e.Settings;
            settings.LocalesDirPath = System.IO.Path.GetFullPath(@"cef\Resources\locales");
            settings.ResourcesDirPath = System.IO.Path.GetFullPath(@"cef\Resources");

            if (_SettingsBuilder == null)
                return;

            _Settings = _SettingsBuilder();
            
            if (_Settings == null)
                return;
            
            settings.SingleProcess = _Settings.SingleProcess;
            settings.UserDataPath = _Settings.UserDataPath;
            settings.RemoteDebuggingPort = _Settings.RemoteDebuggingPort;
            settings.ResourcesDirPath = _Settings.ResourcesDirPath;
            settings.WindowlessRenderingEnabled = _Settings.WindowlessRenderingEnabled;
            settings.BrowserSubprocessPath = _Settings.BrowserSubprocessPath;
        }

        internal static ChromiumFXSession GetSession(Func<CfxSettings> settingsBuilder)
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
