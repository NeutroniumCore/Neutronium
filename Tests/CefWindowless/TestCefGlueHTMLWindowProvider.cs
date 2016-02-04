using MVVM.HTML.Core.JavascriptEngine;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.Test.CefWindowless
{
    public class TestCefGlueHTMLWindowProvider : IHTMLWindowProvider
    {
        public TestCefGlueHTMLWindowProvider(CefFrame iFrame)
        {
            HTMLWindow = new TestCefGlueWindow(iFrame);
        }
        public HTML.Core.Window.IHTMLWindow HTMLWindow
        {
            get;
            private set;
        }

        public HTML.Core.Window.IDispatcher UIDispatcher
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
