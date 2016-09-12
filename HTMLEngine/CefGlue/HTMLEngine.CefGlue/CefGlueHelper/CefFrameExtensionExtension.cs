using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.CefGlue.CefSession;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.CefGlueHelper
{
    public static class CefFrameExtensionExtension
    {
        public static IWebView GetMainContext(this CefFrame @this)
        {
            return CefCoreSessionSingleton.Session.CefApp.GetContext(@this);
        }
    }
}
