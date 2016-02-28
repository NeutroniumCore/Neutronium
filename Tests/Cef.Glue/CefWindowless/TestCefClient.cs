using MVVM.Cef.Glue.Test.Cef.Glue.CefWindowless;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.Test.CefWindowless
{
    public class TestCefClient : CefClient
    {
        private readonly TestCefLoadHandler _TestCefLoadHandler;
        private readonly TestCefRenderHandler _TestCefRenderHandler;
        private readonly TestCefLifeSpanHandler _TestCefLifeSpanHandler;
        private readonly TestDisplayHandler _TestDisplayHandler;

        public TestCefClient()
        {
            _TestCefLoadHandler = new TestCefLoadHandler();
            _TestCefRenderHandler = new TestCefRenderHandler();
            _TestCefLifeSpanHandler = new TestCefLifeSpanHandler();
            _TestDisplayHandler = new TestDisplayHandler();
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

        protected override CefRenderHandler GetRenderHandler()
        {
            return _TestCefRenderHandler;
        }

        protected override CefDisplayHandler GetDisplayHandler()
        {
            return _TestDisplayHandler;
        }
    }
}
