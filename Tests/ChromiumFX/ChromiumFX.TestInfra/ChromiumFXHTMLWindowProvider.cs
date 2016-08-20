using System;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Chromium;

namespace ChromiumFX.TestInfra 
{
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
