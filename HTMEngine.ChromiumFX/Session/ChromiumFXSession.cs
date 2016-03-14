using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chromium;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;

namespace HTMEngine.ChromiumFX.Session
{
    internal class ChromiumFXSession
    {
        private static ChromiumFXSession _Session = null;

        private ChromiumFXSession()
        {
            CfxRuntime.LibCefDirPath = @"cef\Release";

            Chromium.WebBrowser.ChromiumWebBrowser.OnBeforeCfxInitialize += ChromiumWebBrowser_OnBeforeCfxInitialize;
            Chromium.WebBrowser.ChromiumWebBrowser.Initialize();

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
            return null;
        }
    }
}
