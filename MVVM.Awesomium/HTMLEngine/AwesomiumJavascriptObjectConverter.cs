using MVVM.HTML.Core.V8JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awesomium_Core = Awesomium.Core;

namespace MVVM.Awesomium.HTMLEngine
{
    internal class AwesomiumJavascriptObjectConverter : IJavascriptObjectConverter
    {
        private Awesomium_Core.IWebView _IWebView;
        public AwesomiumJavascriptObjectConverter(Awesomium_Core.IWebView iIWebView)
        {
            _IWebView = iIWebView;
        }

        public bool GetSimpleValue(IJavascriptObject value, out object res, Type iTargetType = null)
        {
            res = GetSimpleValue(value.Convert(), iTargetType);
            return res != null;
        }

        public object GetSimpleValue(Awesomium_Core.JSValue ijsvalue, Type iTargetType=null)
        {
            if (ijsvalue.IsString)
                return (string)ijsvalue;

            if (ijsvalue.IsBoolean)
                return (bool)ijsvalue;

            object res =null;

            if (ijsvalue.IsNumber)
            {
                if (ijsvalue.IsInteger)
                    res = (int)ijsvalue;
                else if (ijsvalue.IsDouble)
                    res = (double)ijsvalue;

                if (iTargetType == null)
                    return res;
                else
                    return Convert.ChangeType(res, iTargetType);
            }

            var resdate =  GetDate(ijsvalue);
            if (resdate.HasValue)
                return resdate.Value;

            return null;
        }

        private DateTime? GetDate(Awesomium_Core.JSValue iJSValue)
        {
            if (!iJSValue.IsObject)
                return null;

            Awesomium_Core.JSObject ob = iJSValue;

            if (ob == null)
                return null;

            Awesomium_Core.JSObject ko = _IWebView.ExecuteJavascriptWithResult("ko");
            if ((bool)ko.Invoke("isDate", iJSValue) == false)
                return null;

            int year = (int)ob.Invoke("getFullYear", null);
            int month = (int)ob.Invoke("getMonth", null) + 1;
            int day = (int)ob.Invoke("getDate", null);
            int hour = (int)ob.Invoke("getHours",null);
            int minute = (int)ob.Invoke("getMinutes",null);
            int second = (int)ob.Invoke("getSeconds",null);
            int millisecond = (int)ob.Invoke("getMilliseconds",null);

            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }


      
    }
}
