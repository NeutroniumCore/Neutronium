using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;
using Xilium.CefGlue;

namespace CefGlue.TestInfra.CefWindowless
{
    public class TestCefGlueHTMLWindowProvider : IWebBrowserWindowProvider
    {
        private readonly TestCefClient _TestCefClient;
        public IWebBrowserWindow HTMLWindow { get; }
        public IDispatcher UIDispatcher => new TestUIDispatcher();

        public TestCefGlueHTMLWindowProvider(CefFrame iFrame, TestCefClient cefClient)
        {
            _TestCefClient = cefClient;
            HTMLWindow = new TestCefGlueWindow(iFrame, cefClient);
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
        }
    }
}
