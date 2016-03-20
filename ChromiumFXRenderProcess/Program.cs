using System;
using Chromium;
using Chromium.Event;

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
            handler.OnProcessMessageReceived += Handler_OnProcessMessageReceived;
            app.GetRenderProcessHandler += (sender, args) => args.SetReturnValue(handler);
            int retval = CfxRuntime.ExecuteProcess(app);
            Environment.Exit(retval);
        }

        private static void Handler_OnProcessMessageReceived(object sender, CfxOnProcessMessageReceivedEventArgs e) 
        {
            if (e.Message.Name=="Ended")
                Environment.Exit(0);
        }
    }
}
