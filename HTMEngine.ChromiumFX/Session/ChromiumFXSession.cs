using System;
using Chromium;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;

namespace HTMEngine.ChromiumFX.Session
{
    internal class ChromiumFXSession : IDisposable
    {
        private static ChromiumFXSession _Session = null;

        private ChromiumFXSession()
        {
            CfxRuntime.LibCefDirPath = @"cef\Release";

            ChromiumWebBrowser.OnBeforeCfxInitialize += ChromiumWebBrowser_OnBeforeCfxInitialize;
            ChromiumWebBrowser.Initialize();
        }

        private static void ChromiumWebBrowser_OnBeforeCfxInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            e.Settings.LocalesDirPath = System.IO.Path.GetFullPath(@"cef\Resources\locales");
            e.Settings.ResourcesDirPath = System.IO.Path.GetFullPath(@"cef\Resources");
        }

        internal static ChromiumFXSession GetSession()
        {
            if (_Session != null)
                return _Session;

            _Session = new ChromiumFXSession();
            return _Session;
        }

        public void Dispose() 
        {
            CfxRuntime.Shutdown();
        }
    }
}
