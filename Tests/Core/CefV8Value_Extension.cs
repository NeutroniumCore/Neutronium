using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.Test.Core
{
    public static class CefV8Value_Extension
    {
        public static IJavascriptObject Convert(this CefV8Value value)
        {
            return new CefV8_JavascriptObject(value);
        }

        public static CefV8Value Convert(this IJavascriptObject value)
        {
            return ((CefV8_JavascriptObject) value).RawValue;
        }
    }
}
