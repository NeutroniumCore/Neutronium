using System;
using Chromium.Event;
using Chromium.Remote;
using Chromium.Remote.Event;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    public class ChromiumFxControlWebBrowserWindow : IModernWebBrowserWindow
    {
        private readonly ChromiumWebBrowser _ChromiumWebBrowser;
        private readonly IDispatcher _dispatcher ;
        private readonly IWebSessionLogger _Logger;
        private CfrBrowser _WebBrowser;
        private bool _FirstLoad = true;
        private bool _SendLoadOnContextCreated = false;

        public IWebView MainFrame { get; private set; }
        public Uri Url => _ChromiumWebBrowser.Url;
        public bool IsLoaded  => !_ChromiumWebBrowser.IsLoading;

        public ChromiumFxControlWebBrowserWindow(ChromiumWebBrowser chromiumWebBrowser, IDispatcher dispatcher, IWebSessionLogger logger) 
        {
            _Logger = logger;
            _dispatcher = dispatcher;
            _ChromiumWebBrowser = chromiumWebBrowser;
            _ChromiumWebBrowser.LoadHandler.OnLoadEnd += OnLoadEnd;
            _ChromiumWebBrowser.DisplayHandler.OnConsoleMessage += OnConsoleMessage;
            _ChromiumWebBrowser.OnV8ContextCreated += OnV8ContextCreated;
            _ChromiumWebBrowser.RemoteBrowserCreated += OnChromiumWebBrowser_RemoteBrowserCreated;
            _ChromiumWebBrowser.ContextMenuHandler.OnBeforeContextMenu += OnBeforeContextMenu;
            _ChromiumWebBrowser.RequestHandler.OnRenderProcessTerminated += RequestHandler_OnRenderProcessTerminated;
            _ChromiumWebBrowser.LifeSpanHandler.OnBeforePopup += LifeSpanHandler_OnBeforePopup;
            ChromiumWebBrowser.OnBeforeCommandLineProcessing += ChromiumWebBrowser_OnBeforeCommandLineProcessing;
        }

        private void ChromiumWebBrowser_OnBeforeCommandLineProcessing(CfxOnBeforeCommandLineProcessingEventArgs e)
        {
        }

        private void LifeSpanHandler_OnBeforePopup(object sender, CfxOnBeforePopupEventArgs e)
        {
        }

        private void RequestHandler_OnRenderProcessTerminated(object sender, CfxOnRenderProcessTerminatedEventArgs e) 
        {
            var crashed = Crashed;
            if (crashed != null)
                _dispatcher.RunAsync(() => crashed(this, new BrowserCrashedArgs()));
        }

        private void OnBeforeContextMenu(object sender, CfxOnBeforeContextMenuEventArgs e) 
        {
            e.Model.Clear();
        }

        private void OnChromiumWebBrowser_RemoteBrowserCreated(object sender, RemoteBrowserCreatedEventArgs e) 
        {
            _WebBrowser = e.Browser;
        }

        private void OnV8ContextCreated(object sender, CfrOnContextCreatedEventArgs e)
        {
            MainFrame = new ChromiumFxWebView(e.Browser, _Logger);

            var beforeJavascriptExecuted = BeforeJavascriptExecuted;
            if (beforeJavascriptExecuted == null) 
                return;

            Action<string> excecute = (code) => e.Frame.ExecuteJavaScript(code, String.Empty, 0);
            beforeJavascriptExecuted(this, new BeforeJavascriptExcecutionArgs(excecute));

            if (_SendLoadOnContextCreated)
                SendLoad();
        }

        private void OnConsoleMessage(object sender, CfxOnConsoleMessageEventArgs e)
        {
            ConsoleMessage?.Invoke(this, new ConsoleMessageArgs(e.Message, e.Source, e.Line));
        }

        private void OnLoadEnd(object sender, CfxOnLoadEndEventArgs e)
        {
            if (_FirstLoad) 
            {
                _FirstLoad = false;
                return;
            }

            SendLoad();
        }        

        private void SendLoad()
        {
            var loadEnd = LoadEnd;
            if (loadEnd == null)
                return;

            if (MainFrame == null)
            {
                _ChromiumWebBrowser.ExecuteJavascript("(function(){})()");
                _SendLoadOnContextCreated = true;
                return;
            }

            _SendLoadOnContextCreated = false;
            _dispatcher.RunAsync(() => loadEnd(this, new LoadEndEventArgs(MainFrame)));
        }
        
        public void NavigateTo(Uri path) 
        {
            _ChromiumWebBrowser.LoadUrl(path.AbsolutePath);
        } 

        public event EventHandler<LoadEndEventArgs> LoadEnd;

        public event EventHandler<ConsoleMessageArgs> ConsoleMessage;        
        
        public event EventHandler<BeforeJavascriptExcecutionArgs> BeforeJavascriptExecuted;

        public event EventHandler<BrowserCrashedArgs> Crashed;
    }
}
