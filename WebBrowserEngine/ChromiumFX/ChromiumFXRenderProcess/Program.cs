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
            var returnValue = CfxRuntime.ExecuteProcess();
            Environment.Exit(returnValue);
        }

        private static string CefRepo => (IntPtr.Size == 8) ? "cef64" : "cef";
    }
}
