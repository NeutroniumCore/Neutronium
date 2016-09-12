using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.CefGlue.CefGlueHelper;
using Xilium.CefGlue;

namespace CefGlue.TestInfra.CefWindowless
{
    public class TestCefGlueWindow : IWebBrowserWindow
    {
        private readonly CefFrame _CefFrame;
        private readonly TestCefClient _TestCefClient;
        private IWebView _IWebView;

        public bool IsLoaded => true;
        public Uri Url => new Uri(_CefFrame.Url);

        IWebView IWebBrowserWindow.MainFrame
        {
            get { return _IWebView ?? (_IWebView = _CefFrame.GetMainContext()); }
        }

        public TestCefGlueWindow(CefFrame iFrame, TestCefClient client)
        {
            _CefFrame = iFrame;
            _TestCefClient = client;
            _TestCefClient.TestDisplayHandler.ConsoleMessage += OnConsoleMessage;
        }

        private void OnConsoleMessage(object sender, ConsoleMessageArgs e)
        {
            ConsoleMessage?.Invoke(this, e);
        }

        public void NavigateTo(Uri path)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ConsoleMessageArgs> ConsoleMessage;

        public event EventHandler<LoadEndEventArgs> LoadEnd { add { } remove { } }
       
        public event EventHandler<BrowserCrashedArgs> Crashed { add { } remove { } }
    }
}
