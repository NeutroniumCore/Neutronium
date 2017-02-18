using System;
using Chromium;
using Chromium.WebBrowser.Event;
using Chromium.WebBrowser;

namespace ChromiumFXRenderProcess 
{
    static class Program 
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() 
        {
            CfxRuntime.LibCefDirPath = @"cef\Release";
            ChromiumWebBrowser.OnBeforeCfxInitialize += ChromiumWebBrowser_OnBeforeCfxInitialize;
            int retval = CfxRuntime.ExecuteProcess();
            Environment.Exit(retval);
        }

        private static void ChromiumWebBrowser_OnBeforeCfxInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            e.Settings.LocalesDirPath = System.IO.Path.GetFullPath(@"cef\Resources\locales");
            e.Settings.ResourcesDirPath = System.IO.Path.GetFullPath(@"cef\Resources");
        }
    }
}
