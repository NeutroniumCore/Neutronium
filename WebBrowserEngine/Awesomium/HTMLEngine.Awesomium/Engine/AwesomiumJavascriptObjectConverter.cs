using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.Awesomium.Internal;
using Awesomium_Core = Awesomium.Core;

namespace Neutronium.WebBrowserEngine.Awesomium.Engine
{
    internal class AwesomiumJavascriptObjectConverter : IJavascriptObjectConverter
    {
        private readonly Awesomium_Core.IWebView _WebView;
        private Awesomium_Core.JSObject _Extractor;

        public AwesomiumJavascriptObjectConverter(Awesomium_Core.IWebView webView) 
        {
            _WebView = webView;
        }

        public bool GetSimpleValue(IJavascriptObject value, out object res, Type iTargetType = null) 
        {
            var convert = value.Convert();
            res = GetSimpleValue(convert, iTargetType);
            return (res != null) || (IsObjectNullorUndefined(convert));
        }

        private object GetSimpleValue(Awesomium_Core.JSValue ijsvalue, Type iTargetType = null)
        {
            if (ijsvalue.IsString)
                return (string) ijsvalue;

            if (ijsvalue.IsBoolean)
                return (bool) ijsvalue;

            object res = null;

            if (ijsvalue.IsNumber) 
            {
                if (ijsvalue.IsInteger)
                    res = (int) ijsvalue;
                else if (ijsvalue.IsDouble)
                    res = (double) ijsvalue;

                return (iTargetType == null) ? res : Convert.ChangeType(res, iTargetType);
            }

            var resdate = GetDate(ijsvalue);
            if (resdate.HasValue)
                return resdate.Value;

            return null;
        }

        private Awesomium_Core.JSObject GetExtractor() 
        {
            if (_Extractor != null)
                return _Extractor;

            _Extractor = _WebView.ExecuteJavascriptWithResult("(function() { return { isDate : function(obj) { return obj instanceof Date; },  isNull : function(obj) { return (obj===null) || (obj instanceof Null_reference); }}; })()");

            return _Extractor;
        }

        private DateTime? GetDate(Awesomium_Core.JSValue iJSValue) 
        {
            if (!iJSValue.IsObject)
                return null;

            Awesomium_Core.JSObject ob = iJSValue;

            Awesomium_Core.JSObject extractor = GetExtractor();
            if ((bool) extractor.Invoke("isDate", iJSValue) == false)
                return null;

            int year = (int) ob.Invoke("getFullYear", null);
            int month = (int) ob.Invoke("getMonth", null) + 1;
            int day = (int) ob.Invoke("getDate", null);
            int hour = (int) ob.Invoke("getHours", null);
            int minute = (int) ob.Invoke("getMinutes", null);
            int second = (int) ob.Invoke("getSeconds", null);
            int millisecond = (int) ob.Invoke("getMilliseconds", null);

            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }


        private bool IsObjectNullorUndefined(Awesomium_Core.JSValue value)    
        {
            if ((value.IsNull) || (value.IsUndefined))
                return true;

            var extractor = GetExtractor();
            return ((bool) extractor.Invoke("isNull", value) == true);
        }
    }
}
