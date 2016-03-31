using IntegratedTest.Infra.Windowless;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Xilium.CefGlue;

namespace CefGlue.TestInfra.CefWindowless
{
    public class TestCefGlueHTMLWindowProvider : IHTMLWindowProvider
    {
        public TestCefGlueHTMLWindowProvider(CefFrame iFrame)
        {
            HTMLWindow = new TestCefGlueWindow(iFrame);
        }

        public IHTMLWindow HTMLWindow
        {
            get; private set;
        }

        public IDispatcher UIDispatcher
        {
            get { return new TestIUIDispatcher(); }
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

        public void Dispose()
        {
        }
    }
}
