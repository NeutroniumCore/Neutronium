using System;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WPF.Internal;
using Tests.Infra.WebBrowserEngineTesterHelper.Window;

namespace Tests.Awesomium.Infra
{
    internal class AwesomiumTestHTMLWindowProvider : IWebBrowserWindowProvider
    {
        public IWebBrowserWindow HTMLWindow { get; }
        public IDispatcher UIDispatcher => new WPFUIDispatcher(WpfThread.GetWpfThread().Dispatcher);
        event EventHandler<bool> IWebBrowserWindowProvider.DebugToolOpened { add { } remove { } }

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
