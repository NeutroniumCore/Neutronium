using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.Test.CefWindowless
{
    internal class TestCefLoadHandler : CefLoadHandler
    {
        private TaskCompletionSource<CefBrowser> _LoadEnded = new TaskCompletionSource<CefBrowser>();
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
