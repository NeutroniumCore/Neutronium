using System;
using HTML_WPF.Component;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Tests.Infra.HTMLEngineTesterHelper.Window;

namespace Tests.Awesomium.Infra
{
    internal class AwesomiumTestHTMLWindowProvider : IHTMLWindowProvider
    {
        public IHTMLWindow HTMLWindow { get; }
        public IDispatcher UIDispatcher => new WPFUIDispatcher(WpfThread.GetWpfThread().Dispatcher);

        internal AwesomiumTestHTMLWindowProvider(IWebView webView, Uri path)
        {
            HTMLWindow = new AwesomiumTestHTMLWindow(webView, path);
        }

        public void Show()
        {
        }

        public void Hide()
        {
        }

        public bool OnDebugToolsRequest() => false;

        public void CloseDebugTools() 
        {
        }

        public void Dispose()
        {
        }
    }
}
