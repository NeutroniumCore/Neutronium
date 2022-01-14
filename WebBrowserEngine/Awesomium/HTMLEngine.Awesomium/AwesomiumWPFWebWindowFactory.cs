﻿using System.Threading;
using Awesomium.Core;
using HTMLEngine.Awesomium;
using Neutronium.Core;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.Awesomium
{
    public class AwesomiumWPFWebWindowFactory : IWPFWebWindowFactory
    {
        private static WebConfig _WebConfig = new WebConfig() { RemoteDebuggingPort = 8001, RemoteDebuggingHost = "127.0.0.1" };
        private static WebSession _Session = null;
        public static Thread WebCoreThread { get; set; }

        public string EngineName  => "Chromium 19";
        public string EngineVersion => "19";
        public string Name => "Awesomium";
        public string Environment => null;
        public bool IsModern => false;

        public IWebSessionLogger WebSessionLogger { get; set; }

        static AwesomiumWPFWebWindowFactory()
        {
            WebCore.Started += (o, e) => { WebCoreThread = Thread.CurrentThread; };

            if (!WebCore.IsInitialized) 
                WebCore.Initialize(_WebConfig);
        }

        public AwesomiumWPFWebWindowFactory(IWebSessionLogger logger=null, string webSessionPath =null)
        {
            WebSessionLogger = logger;
            if (_Session != null)
                return;

            _Session = (webSessionPath != null) ?
                            WebCore.CreateWebSession(webSessionPath, new WebPreferences()) :
                            WebCore.CreateWebSession(new WebPreferences());

            WebCore.ShuttingDown += WebCore_ShuttingDown;
        }

        private void WebCore_ShuttingDown(object sender, CoreShutdownEventArgs e) 
        {
            if (e.Exception == null)
                return;

            WebSessionLogger.WebBrowserError(e.Exception, () => e.Cancel = true);
        }

        public IWPFWebWindow Create(bool useNeutroniumSettings) 
        {
            return new AwesomiumWPFWebWindow(_Session, _WebConfig);
        }

        public void Dispose()
        {
            try {
                _Session?.Dispose();
            }
            catch {
            }
            WebCore.Shutdown();
        }
    }
}
