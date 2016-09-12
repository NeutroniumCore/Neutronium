using MVVM.Cef.Glue;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Xilium.CefGlue;

namespace HTMLEngine.CefGlue.CefGlueImplementation
{
    public static class CefV8_Helper
    {
        public static CefV8Value Convert(this IJavascriptObject value)
        {
            return (value as CefV8_JavascriptObject).RawValue;
        }
    }
}
