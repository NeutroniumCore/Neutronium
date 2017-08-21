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

        public uint GetID()
        {
            if (!_JSValue.IsObject)
                return 0;

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

        public void InvokeNoResult(string iFunctionName, IWebView context, params IJavascriptObject[] iparam)
        {
            ((JSObject)_JSValue).Invoke(iFunctionName, iparam.Select(c => c.Convert()).ToArray());
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
            if (webView == null)
                return null;

            return new JSValue_JavascriptObject(webView.ExecuteFunction(_JSValue));
        }

        public IJavascriptObject ExecuteFunction(IWebView webViewGeneric, IJavascriptObject context, params IJavascriptObject[] parameters)
        {
            var webView = webViewGeneric as AwesomiumWebView;
            if (webView == null)
                return null;

            return new JSValue_JavascriptObject(webView.ExecuteFunction(_JSValue, context.Convert(), parameters.Select(el => el.Convert()).ToArray()));
        }

        public void ExecuteFunctionNoResult(IWebView webViewGeneric, IJavascriptObject context, params IJavascriptObject[] parameters)
        {
            var webView = webViewGeneric as AwesomiumWebView;
            if (webView == null)
                return;

            var realContext = (context != null) ? context.Convert() : (JSValue)new JSObject();
            webView.ExecuteFunction(_JSValue, realContext, parameters.Select(el => el.Convert()).ToArray());
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

        public void Bind(string functionName, IWebView webView, Action<IJavascriptObject[]> action)
        {
            Action<string, IJavascriptObject, IJavascriptObject[]> neededAction = (_, __, arg) => action(arg);
            Bind(functionName, webView, neededAction);
        }

        public void BindArgument(string functionName, IWebView webView, Action<IJavascriptObject> action)
        {
            Action<string, IJavascriptObject, IJavascriptObject[]> neededAction = (_, __, arg) => action(arg[0]);
            Bind(functionName, webView, neededAction);
        }

        public void BindArguments(string functionName, IWebView webView, Action<IJavascriptObject, IJavascriptObject> action)
        {
            Action<string, IJavascriptObject, IJavascriptObject[]> neededAction = (_, __, arg) => action(arg[0], arg[1]);
            Bind(functionName, webView, neededAction);
        }

        public void BindArguments(string functionName, IWebView webView, Action<IJavascriptObject, IJavascriptObject, IJavascriptObject> action)
        {
            Action<string, IJavascriptObject, IJavascriptObject[]> neededAction = (_, __, arg) => action(arg[0], arg[1], arg[2]);
            Bind(functionName, webView, neededAction);
        }

        public void BindArguments(string functionName, IWebView webView, Action<IJavascriptObject, IJavascriptObject, IJavascriptObject, IJavascriptObject> action)
        {
            Action<string, IJavascriptObject, IJavascriptObject[]> neededAction = (_, __, arg) => action(arg[0], arg[1], arg[2], arg[3]);
            Bind(functionName, webView, neededAction);
        }
    }
}
