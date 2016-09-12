using System;
using Chromium;
using HTML_WPF.Component;
using Neutronium.Core.JavascriptEngine.Control;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptEngine.Window;
using Tests.Infra.HTMLEngineTesterHelper.Window;

namespace Tests.ChromiumFX.Infra {
    internal class ChromiumFXHTMLWindowProvider : IHTMLWindowProvider 
    {
        private readonly IWebView _webview;
        private readonly CfxClient _CfxClient;
        public IDispatcher UIDispatcher => new WPFUIDispatcher(WpfThread.GetWpfThread().Dispatcher);
        public IHTMLWindow HTMLWindow { get; }

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
