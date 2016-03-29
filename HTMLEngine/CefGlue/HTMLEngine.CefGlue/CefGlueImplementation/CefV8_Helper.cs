using MVVM.Cef.Glue;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
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
