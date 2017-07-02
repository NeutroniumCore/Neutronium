using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chromium;
using Chromium.Remote;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.Convertion;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object 
{
    internal class ChromiumFXJavascriptBase
    {
        protected readonly CfrV8Value _CfrV8Value;

        public bool IsUndefined => _CfrV8Value.IsUndefined;
        public bool IsNull => _CfrV8Value.IsNull;
        public bool IsObject => _CfrV8Value.IsObject;
        public bool IsArray => _CfrV8Value.IsArray;
        public bool IsString => _CfrV8Value.IsString;
        public bool IsNumber => _CfrV8Value.IsDouble || _CfrV8Value.IsUint || _CfrV8Value.IsInt;
        public bool IsBool => _CfrV8Value.IsBool;

        internal ChromiumFXJavascriptBase(CfrV8Value cfrV8Value) 
        {
            _CfrV8Value = cfrV8Value;
        }

        public virtual void Dispose() 
        {
            _CfrV8Value.Dispose();
        }

        public int GetArrayLength() 
        {
            return _CfrV8Value.ArrayLength;
        }

        public bool HasValue(string attributename) 
        {
            return _CfrV8Value.HasValue(attributename);
        }

        public void SetValue(string attributeName, IJavascriptObject element, CreationOption ioption = CreationOption.None) 
        {
            _CfrV8Value.SetValue(attributeName, element.Convert(), (CfxV8PropertyAttribute) ioption);
        }

        public void SetValue(int index, IJavascriptObject element)
        {
            _CfrV8Value.SetValue(index, element.Convert());
        }

        public IJavascriptObject Invoke(string functionName, IWebView context, params IJavascriptObject[] parameters) 
        {
            var function = _CfrV8Value.GetValue(functionName);
            try
            {
                return function.ExecuteFunctionWithContext(context.Convert().V8Context, _CfrV8Value, parameters.Convert()).Convert();
            }
            catch
            {
                return CfrV8Value.CreateUndefined().ConvertBasic();
            }
        }

        public Task<IJavascriptObject> InvokeAsync(string functionName, IWebView context, params IJavascriptObject[] parameters) 
        {
            return Task.FromResult(Invoke(functionName, context, parameters));
        }

        public uint GetID() 
        {
            return (_CfrV8Value.HasValue("_MappedId")) ? _CfrV8Value.GetValue("_MappedId").UintValue : 0;
        }

        public string GetStringValue() 
        {
            return _CfrV8Value.StringValue;
        }

        public double GetDoubleValue() 
        {
            return _CfrV8Value.DoubleValue;
        }

        public bool GetBoolValue() 
        {
            return _CfrV8Value.BoolValue;
        }

        public int GetIntValue() 
        {
            return _CfrV8Value.IntValue;
        }

        public IJavascriptObject GetValue(string ivalue) 
        {
            return _CfrV8Value.GetValue(ivalue).Convert();
        }

        public IJavascriptObject GetValue(int index) 
        {
            return _CfrV8Value.GetValue(index).Convert();
        }

        public IJavascriptObject[] GetArrayElements() 
        {
            if (!_CfrV8Value.IsArray)
                throw new ArgumentException();

            var length = _CfrV8Value.ArrayLength;
            return Enumerable.Range(0, length).Select(_CfrV8Value.GetValue).Convert();
        }

        public IEnumerable<string> GetAttributeKeys()
        {
            var list = new List<string>();
            _CfrV8Value.GetKeys(list);
            return list;
        }
    }
}
