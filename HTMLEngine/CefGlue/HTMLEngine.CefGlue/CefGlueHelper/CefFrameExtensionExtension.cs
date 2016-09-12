using HTMLEngine.CefGlue.CefSession;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Xilium.CefGlue;

namespace HTMLEngine.CefGlue.CefGlueHelper
{
    public static class CefFrameExtensionExtension
    {
        public static IWebView GetMainContext(this CefFrame @this)
        {
            return CefCoreSessionSingleton.Session.CefApp.GetContext(@this);
        }
    }
}
