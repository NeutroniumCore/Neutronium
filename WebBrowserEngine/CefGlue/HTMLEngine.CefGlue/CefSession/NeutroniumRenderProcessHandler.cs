using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.CefSession
{
    internal class NeutroniumRenderProcessHandler : CefRenderProcessHandler
    {
        private readonly CefLoadHandler _CefLoadHandler;
        private readonly NeutroniumCefApp _MVVMCefApp;
        internal NeutroniumRenderProcessHandler(NeutroniumCefApp MVVMCefApp, CefLoadHandler CefLoadHandler)
        {
            _MVVMCefApp = MVVMCefApp;
            _CefLoadHandler = CefLoadHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _CefLoadHandler;
        }

        protected override bool OnBeforeNavigation(CefBrowser browser, CefFrame frame, CefRequest request, CefNavigationType navigation_type, bool isRedirect)
        {
            return false;
        }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            _MVVMCefApp.Associate(browser, frame, context);
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            _MVVMCefApp.Reset(frame);
        }

        protected override void OnWebKitInitialized()
        {
        }

        protected override void OnBrowserDestroyed(CefBrowser browser)
        {
            _MVVMCefApp.Reset(browser);
        }

        //protected override void OnUncaughtException(CefBrowser browser, CefFrame frame, CefV8Context context, CefV8Exception exception, CefV8StackTrace stackTrace)
    }
}
