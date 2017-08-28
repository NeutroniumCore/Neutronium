using System;
using Chromium;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WPF.Internal;
using Tests.Infra.WebBrowserEngineTesterHelper.Window;

namespace Tests.ChromiumFX.Infra {
    internal class ChromiumFXHTMLWindowProvider : IWebBrowserWindowProvider 
    {
        private readonly IWebView _webview;
        private readonly CfxClient _CfxClient;
        public IDispatcher UiDispatcher => new WPFUIDispatcher(WpfThread.GetWpfThread().Dispatcher);
        public IWebBrowserWindow HtmlWindow { get; }
        event EventHandler<DebugEventArgs> IWebBrowserWindowProvider.DebugToolOpened { add { } remove { } }
        public event EventHandler OnDisposed;

        public ChromiumFXHTMLWindowProvider(CfxClient cfxClient, IWebView webview, Uri url) 
        {
            _webview = webview;
            _CfxClient = cfxClient;
            HtmlWindow = new FakeHTMLWindow(cfxClient, webview, url);
        }     

        public void Show() 
        {
        }

        public void Hide() 
        {
        }

        public void Dispose() 
        {
            OnDisposed?.Invoke(this, EventArgs.Empty);
        }

        public bool OnDebugToolsRequest() 
        {
            return false;
        }

        public void CloseDebugTools() 
        {
        }
    }
}
