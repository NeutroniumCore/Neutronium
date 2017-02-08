using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chromium;
using Chromium.Remote;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.Convertion;
using MoreCollection.Set;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object 
{
    internal class ChromiumFXJavascriptObject : IJavascriptObject 
    {
        private readonly CfrV8Value _CfrV8Value;
        private readonly ISet<CfrV8Handler> _Functions = new HybridSet<CfrV8Handler>();

        public bool IsUndefined => _CfrV8Value.IsUndefined;
        public bool IsNull => _CfrV8Value.IsNull;
        public bool IsObject => _CfrV8Value.IsObject;
        public bool IsArray => _CfrV8Value.IsArray;
        public bool IsString => _CfrV8Value.IsString;
        public bool IsNumber => _CfrV8Value.IsDouble || _CfrV8Value.IsUint || _CfrV8Value.IsInt;
        public bool IsBool => _CfrV8Value.IsBool;

        public ChromiumFXJavascriptObject(CfrV8Value cfrV8Value) 
        {
            _CfrV8Value = cfrV8Value?? CfrV8Value.CreateUndefined();
        }

        internal CfrV8Value GetRaw() 
        {
            return _CfrV8Value;
        }

        public void Dispose() 
        {
            _CfrV8Value.Dispose();
            _Functions.Clear();
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
            if (function.IsUndefined)
                return CfrV8Value.CreateUndefined().Convert();
            return function.ExecuteFunctionWithContext(context.Convert().V8Context, _CfrV8Value, parameters.Convert()).Convert();
        }

        public Task<IJavascriptObject> InvokeAsync(string functionName, IWebView context, params IJavascriptObject[] parameters) 
        {
            return Task.FromResult(Invoke(functionName, context, parameters));
        }

        public void Bind(string functionName, IWebView context, Action<string, IJavascriptObject, IJavascriptObject[]> action) 
        {
            lock (this) 
            {
                 var cfrV8Handler = action.Convert(functionName);
                _Functions.Add(cfrV8Handler);
                var func = CfrV8Value.CreateFunction(functionName, cfrV8Handler);
                _CfrV8Value.SetValue(functionName, func, CfxV8PropertyAttribute.ReadOnly | CfxV8PropertyAttribute.DontDelete);
            }           
        }

        public IJavascriptObject ExecuteFunction(IWebView context) 
        {
            return _CfrV8Value.ExecuteFunction(_CfrV8Value, new CfrV8Value[0]).Convert();
        }

        public bool HasRelevantId() 
        {
            return _CfrV8Value.HasValue("_MappedId");
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
