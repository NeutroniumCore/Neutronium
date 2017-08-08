using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.CefGlue.CefGlueHelper;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.CefGlueImplementation
{
    public class CefV8_JavascriptObject : IJavascriptObject
    {
        private readonly CefV8Value _CefV8Value;
        public CefV8_JavascriptObject(CefV8Value iCefV8Value)
        {
            _CefV8Value = iCefV8Value;
        }

        internal CefV8Value RawValue { get { return _CefV8Value; } }

        #region Converters

        internal static CefV8Value Convert(IJavascriptObject iIJavascriptObject)
        {
            return (iIJavascriptObject as CefV8_JavascriptObject)._CefV8Value;
        }

        internal static CefV8Value[] Convert(IJavascriptObject[] iIJavascriptObject)
        {
            return iIJavascriptObject.Select(p => Convert(p)).ToArray();
        }

        internal static IJavascriptObject Convert(CefV8Value value)
        {
            return new CefV8_JavascriptObject(value);
        }

        internal static IJavascriptObject[] Convert(CefV8Value[] value)
        {
            return value.Select(p => Convert(p)).ToArray();
        }

        private static CefV8PropertyAttribute Convert(CreationOption value)
        {
            return (CefV8PropertyAttribute)value;
        }

        #endregion

        public bool IsUndefined
        {
            get { return _CefV8Value.IsUndefined; }
        }

        public bool IsNull
        {
            get { return _CefV8Value.IsNull; }
        }

        public bool IsObject
        {
            get { return _CefV8Value.IsObject; }
        }

        public bool IsArray
        {
            get { return _CefV8Value.IsArray; }
        }

        public bool IsString
        {
            get { return _CefV8Value.IsString; }
        }

        public bool IsNumber
        {
            get { return _CefV8Value.IsInt || _CefV8Value.IsDouble; }
        }

        public bool IsBool 
        {
            get { return _CefV8Value.IsBool; }
        }

        public bool IsDate
        {
            get { return _CefV8Value.IsDate; }
        }

        public int GetArrayLength()
        {
            return _CefV8Value.GetArrayLength();
        }

        public bool HasValue(string attributename)
        {
            return _CefV8Value.HasValue(attributename);
        }

        public IJavascriptObject Invoke(string functionName, IWebView context, params IJavascriptObject[] parameters)
        {
            return new CefV8_JavascriptObject(_CefV8Value.Invoke(functionName, context, Convert(parameters)));
        }

        public void InvokeNoResult(string functionName, IWebView context, params IJavascriptObject[] parameters)
        {
            _CefV8Value.Invoke(functionName, context, Convert(parameters));
        }

        public async Task<IJavascriptObject> InvokeAsync(string iFunctionName, IWebView iContext, params IJavascriptObject[] iparam)
        {
            return Convert( await  _CefV8Value.InvokeAsync(iFunctionName, iContext, Convert(iparam)));
        }

        public void Bind(string iFunctionName, IWebView iContext, Action<string, IJavascriptObject, IJavascriptObject[]> action)
        {
            Action<string, CefV8Value, CefV8Value[]> Converted = (s, t, p) => action(s, Convert(t), Convert(p));
            _CefV8Value.Bind(iFunctionName, iContext, Converted);
        }

        public void SetValue(string AttributeName, IJavascriptObject element, CreationOption option = CreationOption.None)
        {
            _CefV8Value.SetValue(AttributeName, Convert(element),Convert(option));
        }

        public void SetValue(int index, IJavascriptObject element)
        {
            _CefV8Value.SetValue(index, Convert(element));
        }

        public string GetStringValue()
        {
            return _CefV8Value.GetStringValue();
        }

        public int GetIntValue()
        {
            return _CefV8Value.GetIntValue();
        }

        public double GetDoubleValue()
        {
            return _CefV8Value.GetDoubleValue();
        }

        public bool GetBoolValue()
        {
            return _CefV8Value.GetBoolValue();
        }

        public IJavascriptObject GetValue(string ivalue)
        {
            return Convert(_CefV8Value.GetValue(ivalue));
        }

        public IJavascriptObject GetValue(int ivalue)
        {
            return Convert(_CefV8Value.GetValue(ivalue));
        }

        public IJavascriptObject[] GetArrayElements()
        {
            return Convert(_CefV8Value.GetArrayElements());
        }

        public IJavascriptObject ExecuteFunction(IWebView context)
        {
            return Convert(_CefV8Value.ExecuteFunction());
        }

        public IJavascriptObject ExecuteFunction(IWebView webView, IJavascriptObject context, params IJavascriptObject[] parameters)
        {
            return Convert(_CefV8Value.ExecuteFunction(context?.Convert(), Convert(parameters)));
        }

        public void ExecuteFunctionNoResult(IWebView webView, IJavascriptObject context, params IJavascriptObject[] parameters)
        {
            _CefV8Value.ExecuteFunction(context?.Convert(), Convert(parameters));
        }

        public void Dispose()
        {
            _CefV8Value.Dispose();
        }

        public bool HasRelevantId()
        {
            return _CefV8Value.HasValue(NeutroniumConstants.ObjectId);
        }

        public uint GetID()
        {
            return (_CefV8Value.HasValue(NeutroniumConstants.ObjectId)) ? _CefV8Value.GetValue(NeutroniumConstants.ObjectId).GetUIntValue() : 0;
        }

        public IEnumerable<string> GetAttributeKeys()
        {
            return _CefV8Value.GetKeys();
        }
    }
}