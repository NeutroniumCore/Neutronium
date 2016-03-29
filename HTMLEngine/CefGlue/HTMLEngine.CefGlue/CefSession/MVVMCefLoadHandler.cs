using Xilium.CefGlue;

namespace HTMLEngine.CefGlue.CefSession
{
    public class MVVMCefLoadHandler : CefLoadHandler
    {
        private readonly MVVMCefApp _MVVMCefApp;
        public MVVMCefLoadHandler(MVVMCefApp mvvmCefApp)
        {
            _MVVMCefApp = mvvmCefApp;
        }

        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            _MVVMCefApp.EnsureLoaded(frame);
        }

        //protected override void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)

        protected override void OnLoadStart(CefBrowser browser, CefFrame frame)
        {
            _MVVMCefApp.OnLoadStart(browser, frame);
        }
    }
}
