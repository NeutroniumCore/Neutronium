using System.Threading.Tasks;
using Xilium.CefGlue;

namespace CefGlue.TestInfra.CefWindowless
{
    internal class TestCefLoadHandler : CefLoadHandler
    {
        private readonly TaskCompletionSource<CefBrowser> _LoadEnded = new TaskCompletionSource<CefBrowser>();
        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            base.OnLoadEnd(browser, frame, httpStatusCode);
            _LoadEnded.TrySetResult(browser);
        }

        internal Task<CefBrowser> GetLoadedBroserAsync()
        {
            return _LoadEnded.Task;
        }
    }
}
