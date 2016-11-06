using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awesomium.Core;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.Awesomium.Internal;
using IWebView = Neutronium.Core.WebBrowserEngine.JavascriptObject.IWebView;

namespace Neutronium.WebBrowserEngine.Awesomium.Engine
{
    internal class JSValue_JavascriptObject : IJavascriptObject
    {
        private JSValue _JSValue;
        public JSValue_JavascriptObject(JSValue iJSValue)
        {
            _JSValue = iJSValue;
        }

        public JSValue JSValue { get { return _JSValue; } }

        public bool HasRelevantId()
        {
            if (!_JSValue.IsObject)
                return false;

            var jso = (JSObject)_JSValue;
            return (jso.RemoteId != 0);
        }

        public uint GetID()
        {
            JSObject jso = _JSValue;
            return jso.RemoteId;
        }

        public bool IsUndefined
        {
            get { return _JSValue.IsUndefined; }
        }

        public bool IsNull
        {
            get { return _JSValue.IsNull; }
        }

        public bool IsObject
        {
            get { return _JSValue.IsObject; }
        }

        public bool IsArray
        {
            get { return _JSValue.IsArray; }
        }

        public bool IsString
        {
            get { return _JSValue.IsString; }
        }

        public bool IsNumber
        {
            get { return _JSValue.IsNumber; }
        }

        public bool IsBool
        {
            get { return _JSValue.IsBoolean; }
        }

        public int GetArrayLength()
        {
            return ((JSValue[])_JSValue).Length;
        }

        public bool HasValue(string attributename)
        {
            if (!IsObject)
                return false;

            return ((JSObject)_JSValue).HasProperty(attributename);
        }

        public IJavascriptObject Invoke(string iFunctionName, IWebView context, params IJavascriptObject[] iparam)
        {
            var res = ((JSObject)_JSValue).Invoke(iFunctionName, iparam.Select(c => c.Convert()).ToArray());
            return res.Convert();
        }

        public Task<IJavascriptObject> InvokeAsync(string iFunctionName, IWebView iContext, params IJavascriptObject[] iparam)
        {
            return Task.FromResult(Invoke(iFunctionName, iContext, iparam));
        }

        public void Bind(string iFunctionName, IWebView iContext, Action<string, IJavascriptObject, IJavascriptObject[]> action)
        {
            JSObject ob = _JSValue;
            ob.Bind(iFunctionName, false, (o, e) => { action(iFunctionName, null, e.Arguments.Select(el => el.Convert()).ToArray()); });
        }

        public void SetValue(string AttributeName, IJavascriptObject element, CreationOption ioption = CreationOption.None)
        {
            ((JSObject)_JSValue)[AttributeName] = element.Convert();
        }


        public void SetValue(int index, IJavascriptObject element)
        {
            ((JSValue[])_JSValue)[index] = element.Convert();
        }

        public string GetStringValue()
        {
            return (string)_JSValue;
        }

        public double GetDoubleValue()
        {
            return (double)_JSValue;
        }

        public bool GetBoolValue()
        {
            return (bool)_JSValue;
        }

        public int GetIntValue()
        {
            return (int)_JSValue;
        }

        public IJavascriptObject ExecuteFunction(IWebView context)
        {
            var webView = context as AwesomiumWebView;
            if (context == null)
                return null;

            return new JSValue_JavascriptObject(webView.ExecuteFunction(_JSValue));
        }

        public IJavascriptObject GetValue(string ivalue)
        {
            return ((JSObject)_JSValue)[ivalue].Convert();
        }

        public IJavascriptObject GetValue(int ivalue)
        {
            var ar = (JSValue[])_JSValue;
            return ar[ivalue].Convert();
        }

        public IJavascriptObject[] GetArrayElements()
        {
            JSValue[] ar;
            try
            {
                ar = (JSValue[])_JSValue;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException();
            }
            return ar.Cast<JSValue>().Select(el => el.Convert()).ToArray();
        }

        public void Dispose()
        {
        }

        public IEnumerable<string> GetAttributeKeys()
        {
            return ((JSObject)_JSValue).GetPropertyNames();
        }
    }
}
