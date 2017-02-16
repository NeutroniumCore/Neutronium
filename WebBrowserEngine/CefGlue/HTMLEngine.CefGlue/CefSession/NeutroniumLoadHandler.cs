using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.CefSession
{
    public class NeutroniumLoadHandler : CefLoadHandler
    {
        private readonly NeutroniumCefApp _MVVMCefApp;
        public NeutroniumLoadHandler(NeutroniumCefApp mvvmCefApp)
        {
            _MVVMCefApp = mvvmCefApp;
        }

        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            _MVVMCefApp.EnsureLoaded(frame);
        }

        protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
        {
            _MVVMCefApp.OnLoadStart(browser, frame);
        }
    }
}
