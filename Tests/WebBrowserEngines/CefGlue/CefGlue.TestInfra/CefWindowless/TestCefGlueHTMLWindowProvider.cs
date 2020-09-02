using System;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WPF.Internal;
using Tests.Infra.WebBrowserEngineTesterHelper.Window;
using Xilium.CefGlue;

namespace CefGlue.TestInfra.CefWindowless
{
    public class TestCefGlueHTMLWindowProvider : IWebBrowserWindowProvider
    {
        private readonly TestCefClient _TestCefClient;

        event EventHandler<DebugEventArgs> IWebBrowserWindowProvider.DebugToolOpened { add { } remove { } }
        public event EventHandler OnDisposed;
        public IWebBrowserWindow HtmlWindow { get; }
        public IUiDispatcher UiDispatcher => new WPFUIDispatcher(WpfThread.GetWpfThread().Dispatcher);

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
