using System;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace ChromiumFX.TestInfra 
{
    internal class ChromiumFXHTMLWindowProvider : IHTMLWindowProvider 
    {
        private readonly IWebView _webview;                  
        public ChromiumFXHTMLWindowProvider(IWebView webview, Uri url) 
        {
            _webview = webview;
            HTMLWindow = new FakeHTMLWindow(webview, url);
        }

        public IHTMLWindow HTMLWindow 
        {
            get; private set;
        }

        public IDispatcher UIDispatcher 
        {
            get { return new WPFUIDispatcher(WpfThread.GetWpfThread().Dispatcher); }
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
    }
}
