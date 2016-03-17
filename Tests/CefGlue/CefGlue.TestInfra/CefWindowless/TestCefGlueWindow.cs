using System;
using MVVM.Cef.Glue.CefGlueHelper;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Xilium.CefGlue;

namespace CefGlue.TestInfra.CefWindowless
{
    public class TestCefGlueWindow : IHTMLWindow
    {
        private CefFrame _CefFrame;
        public TestCefGlueWindow(CefFrame iFrame)
        {
            _CefFrame = iFrame;
        }

        public bool IsLoaded
        {
            get { return true; }
        }

        public event EventHandler<LoadEndEventArgs> LoadEnd 
        { 
            add { } remove { } 
        }

        IWebView IHTMLWindow.MainFrame
        {
            get { return _CefFrame.GetMainContext(); }
        }

        public void NavigateTo(Uri path)
        {
            throw new NotImplementedException();
        }

        public Uri Url
        {
            get { return new Uri(_CefFrame.Url); }
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
