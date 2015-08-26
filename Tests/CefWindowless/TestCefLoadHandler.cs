using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.Test.CefWindowless
{
    internal class TestCefLoadHandler : CefLoadHandler
    {
        private TaskCompletionSource<CefBrowser> _LoadEnded = new TaskCompletionSource<CefBrowser>();
        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            base.OnLoadEnd(browser, frame, httpStatusCode);
            _LoadEnded.TrySetResult(browser);
        }

        protected override void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        {
            base.OnLoadError(browser, frame, errorCode, errorText, failedUrl);
        }

        internal Task<CefBrowser> GetLoadedBroserAsync()
        {
            return _LoadEnded.Task;
        }
    }
}
