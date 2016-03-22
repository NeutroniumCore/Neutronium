using Awesomium.Core;
using MVVM.Awesomium.HTMLEngine;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.Awesomium
{
    internal static class AwesomiumConverter
    {
        public static IJavascriptObject Convert(this JSValue ivalue)
        {
            return new JSValue_JavascriptObject(ivalue);
        }

        public static IJavascriptObject Convert(this JSObject ivalue)
        {
            return new JSValue_JavascriptObject(ivalue);
        }

        public static JSValue Convert(this IJavascriptObject ivalue)
        {
            return ((JSValue_JavascriptObject) ivalue).JSValue;
        }
    }
}
