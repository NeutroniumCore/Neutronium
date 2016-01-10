using MVVM.Cef.Glue.CefGlueHelper;
using MVVM.HTML.Core.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.Test.CefWindowless
{
    public class TestCefGlueWindow : IHTMLWindow
    {
        private CefFrame _CefFrame;
        public TestCefGlueWindow(CefFrame iFrame)
        {
            _CefFrame = iFrame;
        }

        public CefFrame MainFrame
        {
            get { return _CefFrame; }
        }

        public bool IsLoaded
        {
            get { return true; }
        }

        public event EventHandler<LoadEndEventArgs> LoadEnd;

        public IDispatcher GetDispatcher()
        {
            return new TestIUIDispatcher();
        }

        HTML.Core.V8JavascriptObject.IWebView IHTMLWindow.MainFrame
        {
            get { return _CefFrame.GetMainContext(); }
        }
 
        public void NavigateTo(string path)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<HTML.Core.JavascriptEngine.ConsoleMessageArgs> ConsoleMessage;
    }
}
