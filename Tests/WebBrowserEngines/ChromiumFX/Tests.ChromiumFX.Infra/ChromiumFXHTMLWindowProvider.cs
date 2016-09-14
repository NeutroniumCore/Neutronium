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
        public IDispatcher UIDispatcher => new WPFUIDispatcher(WpfThread.GetWpfThread().Dispatcher);
        public IWebBrowserWindow HTMLWindow { get; }

        public ChromiumFXHTMLWindowProvider(CfxClient cfxClient, IWebView webview, Uri url) 
        {
            _webview = webview;
            _CfxClient = cfxClient;
            HTMLWindow = new FakeHTMLWindow(cfxClient, webview, url);
        }     

        public void Show() 
        {
        }

        public void Hide() 
        {
        }

        public void Dispose() 
        {
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
