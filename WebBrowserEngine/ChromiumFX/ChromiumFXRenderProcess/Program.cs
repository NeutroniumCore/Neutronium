using System;
using Chromium;

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
            CfxRuntime.LibCefDirPath = $@"{CefRepo}\Release";
            var retval = CfxRuntime.ExecuteProcess();
            Environment.Exit(retval);
        }

        private static string CefRepo => (IntPtr.Size == 8) ? "cef64" : "cef";
    }
}
