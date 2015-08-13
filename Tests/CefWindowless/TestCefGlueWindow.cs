using CefGlue.Window;
using MVVM.CEFGlue.CefGlueHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.Test.CefWindowless
{
    public class TestCefGlueWindow : ICefGlueWindow
    {
        private CefFrame _CefFrame;
        //private CefV8Context _CefV8Context;
        public TestCefGlueWindow(CefFrame iFrame)
        {
            _CefFrame = iFrame;
            //_CefV8Context = iContext.Context;
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
    }
}
