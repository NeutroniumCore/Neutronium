using CefGlue.Window;
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
        private CefV8Context _CefV8Context;
        public TestCefGlueWindow(CefFrame iFrame, CefV8Context iContext )
        {
            _CefFrame = iFrame;
            _CefV8Context = iContext;
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

        public IUIDispatcher GetDispatcher()
        {
            return new TestIUIDispatcher();
        }
    }
}
