using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Tests.Awesomium.Infra
{
    internal class AwesomiumTestHTMLWindow : IWebBrowserWindow
    {
        public IWebView MainFrame { get; }
        public Uri Url { get; }
        public bool IsLoaded => true;

        internal AwesomiumTestHTMLWindow(IWebView webView, Uri path)
        {
            MainFrame = webView;
            Url = path;
        }

        public void NavigateTo(Uri path)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ConsoleMessageArgs> ConsoleMessage
        {
            add { } remove { }
        }

        public event EventHandler<BrowserCrashedArgs> Crashed
        {
            add { } remove { }
        }

        public event EventHandler<LoadEndEventArgs> LoadEnd
        {
            add { } remove { }
        }
    }
}
