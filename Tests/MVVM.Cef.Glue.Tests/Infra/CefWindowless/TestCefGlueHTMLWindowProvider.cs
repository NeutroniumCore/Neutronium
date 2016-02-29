using IntegratedTest.Windowless;
using MVVM.HTML.Core.JavascriptEngine;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.Test.CefWindowless
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

        public void Dispose()
        {
        }
    }
}
