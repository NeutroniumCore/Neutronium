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
            CfxRuntime.LibCefDirPath = @"cef\Release";
            var app = new CfxApp();
            var handler = new CfxRenderProcessHandler();
            app.GetRenderProcessHandler += (sender, args) => args.SetReturnValue(handler);
            int retval = CfxRuntime.ExecuteProcess(app);
            Environment.Exit(retval);
        }
    }
}
