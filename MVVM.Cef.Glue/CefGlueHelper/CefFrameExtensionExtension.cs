using Xilium.CefGlue;
using MVVM.Cef.Glue.CefSession;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.Cef.Glue.CefGlueHelper
{
    public static class CefFrameExtensionExtension
    {
        public static IWebView GetMainContext(this CefFrame @this)
        {
            return CefCoreSessionSingleton.Session.CefApp.GetContext(@this);
        }
    }
}
