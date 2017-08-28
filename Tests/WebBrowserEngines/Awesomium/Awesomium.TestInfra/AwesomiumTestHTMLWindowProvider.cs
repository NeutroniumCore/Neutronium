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
        public IWebBrowserWindow HtmlWindow { get; }
        public IDispatcher UiDispatcher => new WPFUIDispatcher(WpfThread.GetWpfThread().Dispatcher);
        event EventHandler<DebugEventArgs> IWebBrowserWindowProvider.DebugToolOpened { add { } remove { } }
        public event EventHandler OnDisposed;

        internal AwesomiumTestHTMLWindowProvider(IWebView webView, Uri path)
        {
            HtmlWindow = new AwesomiumTestHTMLWindow(webView, path);
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
            OnDisposed?.Invoke(this, EventArgs.Empty);
        }
    }
}
