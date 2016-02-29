using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using System;

namespace MVVM.Awesomium.Tests
{
    internal class AwesomiumTestHTMLWindow : IHTMLWindow
    {
        internal AwesomiumTestHTMLWindow(IWebView webView, string path)
        {
            MainFrame = webView;
            Url = path;
        }

        public IWebView MainFrame
        {
            get; private set;
        }

        public void NavigateTo(string path)
        {
            throw new NotImplementedException();
        }

        public string Url
        {
            get; private set;
        }

        public bool IsLoaded
        {
            get { return true; }
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
