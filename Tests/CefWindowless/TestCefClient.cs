using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.Test.CefWindowless
{
    public class TestCefClient : CefClient
    {
        private TestCefLoadHandler _TestCefLoadHandler;
        private TestCefRenderHandler _TestCefRenderHandler;
        private TestCefLifeSpanHandler _TestCefLifeSpanHandler;
        public TestCefClient()
        {
            _TestCefLoadHandler = new TestCefLoadHandler();
            _TestCefRenderHandler = new TestCefRenderHandler();
            _TestCefLifeSpanHandler = new TestCefLifeSpanHandler();
        }

        internal Task<CefBrowser> GetLoadedBroserAsync()
        {
            return _TestCefLoadHandler.GetLoadedBroserAsync();
        }
        
        protected override CefLoadHandler GetLoadHandler()
        {
            return _TestCefLoadHandler;
        }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _TestCefLifeSpanHandler;
        }

        protected override CefDisplayHandler GetDisplayHandler()
        {
            return base.GetDisplayHandler();
        }

        protected override CefContextMenuHandler GetContextMenuHandler()
        {
            return base.GetContextMenuHandler();
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return _TestCefRenderHandler;
        }
    }
}
