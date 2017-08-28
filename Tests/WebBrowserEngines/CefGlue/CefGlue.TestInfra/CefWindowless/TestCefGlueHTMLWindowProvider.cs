using System;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;
using Xilium.CefGlue;

namespace CefGlue.TestInfra.CefWindowless
{
    public class TestCefGlueHTMLWindowProvider : IWebBrowserWindowProvider
    {
        private readonly TestCefClient _TestCefClient;

        event EventHandler<DebugEventArgs> IWebBrowserWindowProvider.DebugToolOpened { add { } remove { } }
        public event EventHandler OnDisposed;
        public IWebBrowserWindow HtmlWindow { get; }
        public IDispatcher UiDispatcher => new NullUIDispatcher();

        public TestCefGlueHTMLWindowProvider(CefFrame iFrame, TestCefClient cefClient)
        {
            _TestCefClient = cefClient;
            HtmlWindow = new TestCefGlueWindow(iFrame, cefClient);
        }

        public void Show()
        {
        }

        public void Hide()
        {
        }

        public bool OnDebugToolsRequest() 
        {
            return false;
        }

        public void CloseDebugTools() 
        {
        }

        public void Dispose()
        {
            OnDisposed?.Invoke(this, EventArgs.Empty);
        }
    }
}
