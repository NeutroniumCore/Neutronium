using System.Diagnostics;
using System.Threading;
using Awesomium.Core;
using HTML_WPF.Component;
using MVVM.HTML.Core;

namespace HTMLEngine.Awesomium
{
    public class AwesomiumWPFWebWindowFactory : IWPFWebWindowFactory
    {
        private static WebConfig _WebConfig = new WebConfig() { RemoteDebuggingPort = 8001, RemoteDebuggingHost = "127.0.0.1" };

        private static WebSession _Session = null;

        public string EngineName 
        {
            get { return "Chromium 19"; }
        }

        public string Name 
        {
            get { return "Awesomium"; }
        }

        static AwesomiumWPFWebWindowFactory()
        {
            WebCore.Started += (o, e) => { WebCoreThread = Thread.CurrentThread; };

            if (!WebCore.IsInitialized) 
                WebCore.Initialize(_WebConfig);
        }

        public static Thread WebCoreThread { get; internal set; }

        public AwesomiumWPFWebWindowFactory(string webSessionPath=null) 
        {
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

            WebSessionWatcher.LogCritical("Critical: WebCore ShuttingDown!!");
            Trace.WriteLine(string.Format("HTMLEngine.Awesomium : WebCoreShutting Down, due to exception: {0}", e.Exception));
            WebSessionWatcher.OnSessionError(e.Exception, () => e.Cancel = true);
        }

        public IWPFWebWindow Create() 
        {
            return new AwesomiumWPFWebWindow(_Session, _WebConfig);
        }

        public IWebSessionWatcher WebSessionWatcher { get; set; }

        public void Dispose()
        {
            if (_Session != null)
                _Session.Dispose();

            WebCore.Shutdown();
        }
    }
}
