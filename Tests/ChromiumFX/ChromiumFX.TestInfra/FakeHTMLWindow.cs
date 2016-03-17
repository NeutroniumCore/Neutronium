using System;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace ChromiumFX.TestInfra 
{
    internal class FakeHTMLWindow : IHTMLWindow
    {
        private readonly IWebView _Webview;
        private Uri _Uri;
        public FakeHTMLWindow(IWebView webview, Uri url) 
        {
            _Uri = url;
            _Webview = webview;
        }

        public IWebView MainFrame 
        {
            get { return _Webview; }
        }

        public void NavigateTo(Uri path) 
        {
            throw new NotImplementedException();
        }

        public Uri Url 
        {
            get { return _Uri; }
        }

        public bool IsLoaded 
        {
            get { return true; }
        }

        public event EventHandler<LoadEndEventArgs> LoadEnd 
        {
            add { } remove { }
        }

        public event EventHandler<ConsoleMessageArgs> ConsoleMessage 
        {
            add { } remove { }
        }

        public event EventHandler<BrowserCrashedArgs> Crashed 
        {
            add { } remove { }
        }
    }
}
