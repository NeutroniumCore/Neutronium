using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.Awesomium.Engine;

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

        public bool IsTypeBasic(Type type) 
        {
            return AwesomiumJavascriptObjectFactory.IsTypeConvertible(type);
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
