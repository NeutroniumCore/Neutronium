using Xilium.CefGlue;

using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.Cef.Glue.CefSession;


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
