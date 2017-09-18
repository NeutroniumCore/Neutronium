using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Xilium.CefGlue;
using MoreCollection.Extensions;

namespace Neutronium.WebBrowserEngine.CefGlue.CefGlueImplementation
{
    internal class CefV8_Factory : IJavascriptObjectFactory
    {
        private static uint _Count = 0;
        private readonly IWebView _CefV8WebView;

        private readonly Lazy<CefV8Value> _ObjectWithConstructorBuilder;

        public CefV8_Factory(IWebView cefV8WebView)
        {
            _CefV8WebView = cefV8WebView;
            _ObjectWithConstructorBuilder = new Lazy<CefV8Value>(GetObjectWithConstructorBuilder);
        }

        private CefV8Value GetObjectWithConstructorBuilder()
        {
            var builderScript = @"(function(){
                function createObjectWithConstructor(id, constructor){
                    const allArgs = Array.from(arguments)
                    const args = allArgs.slice(2)
                    return new constructor(id, ...args)
                }
                return {
                    createObjectWithConstructor
                };
            }())";
            return Eval(builderScript).GetValue("createObjectWithConstructor");
        }

        private CefV8Value Eval(string code)
        {
            IJavascriptObject res;
            _CefV8WebView.Eval(code, out res);
            return res.Convert();
        }

        public IEnumerable<IJavascriptObject> CreateFromExcecutionCode(IEnumerable<string> @from)
        {
            foreach (var code in from)
            {
                yield return CreateObject(code);
            }
        }

        public IJavascriptObject CreateNull()
        {
            return new CefV8_JavascriptObject(CefV8Value.CreateNull());
        }

        public IJavascriptObject CreateUndefined()
        {
            return new CefV8_JavascriptObject(CefV8Value.CreateUndefined());
        }

        public IJavascriptObject CreateObject()
        {
            return UpdateObject(CefV8Value.CreateObject(null));
        }

        public IJavascriptObject CreateObject(bool readOnly)
        {
            return UpdateObject(CefV8Value.CreateObject(null), readOnly);
        }

        public IEnumerable<IJavascriptObject> CreateObjects(int readWrite, int readOnlyNumber)
        {
            for (var i = 0; i < readWrite; i++)
            {
                yield return CreateObject(false);
            }
            for (var i = 0; i < readOnlyNumber; i++)
            {
                yield return CreateObject(true);
            }
        }

        public IJavascriptObject CreateInt(int value)
        {
            return new CefV8_JavascriptObject(CefV8Value.CreateInt(value));
        }

        public IJavascriptObject CreateUint(uint value)
        {
            return new CefV8_JavascriptObject(CefV8Value.CreateUInt(value));
        }

        public IJavascriptObject CreateString(string value)
        {
            return new CefV8_JavascriptObject(CefV8Value.CreateString(value));
        }

        public IJavascriptObject CreateBool(bool value)
        {
            return new CefV8_JavascriptObject(CefV8Value.CreateBool(value));
        }

        public IJavascriptObject CreateDateTime(DateTime value)
        {
            return new CefV8_JavascriptObject(CefV8Value.CreateDate(value));
        }

        public IJavascriptObject CreateDouble(double value)
        {
            return new CefV8_JavascriptObject(CefV8Value.CreateDouble(value));
        }

        public IJavascriptObject CreateArray(IEnumerable<IJavascriptObject> iCollection)
        {
            var col = iCollection.ToList();
            var res = CefV8Value.CreateArray(col.Count);
            col.ForEach((el, i) => res.SetValue(i, el.Convert()));
            return UpdateObject(res);
        }

        public IJavascriptObject CreateArray(int size)
        {
            var res = CefV8Value.CreateArray(size);
            return UpdateObject(res);
        }

        public IEnumerable<IJavascriptObject> CreateArrays(int number)
        {
            for (var i = 0; i < number; i++)
            {
                yield return CreateArray(0);
            }
        }

        private static void SetAttribute(CefV8Value value, string name, CefV8Value propertyValue)
        {
            value.SetValue(name, propertyValue, CefV8PropertyAttribute.ReadOnly | CefV8PropertyAttribute.DontEnum | CefV8PropertyAttribute.DontDelete);
        }

        private void BasicUpdateObject(CefV8Value res, bool? readOnly = null)
        {
            if (res == null)
                return;
            SetAttribute(res, NeutroniumConstants.ObjectId, CefV8Value.CreateUInt(_Count++));

            if (!readOnly.HasValue)
                return;
            SetAttribute(res, NeutroniumConstants.ReadOnlyFlag, CefV8Value.CreateBool(readOnly.Value));
        }

        private CefV8_JavascriptObject UpdateObject(CefV8_JavascriptObject res, bool? readOnly = null)
        {
            BasicUpdateObject(res.RawValue, readOnly);
            return res;
        }

        private CefV8_JavascriptObject UpdateObject(CefV8Value res, bool? readOnly = null)
        {
            BasicUpdateObject(res, readOnly);
            return new CefV8_JavascriptObject(res);
        }

        public IJavascriptObject CreateObject(string iCreationCode)
        {
            return _CefV8WebView.Evaluate(() =>
            {
                IJavascriptObject res;
                _CefV8WebView.Eval(iCreationCode, out res);
                return UpdateObject(res as CefV8_JavascriptObject);
            });
        }

        public IEnumerable<IJavascriptObject> CreateObjectsFromContructor(int number, IJavascriptObject constructor, params IJavascriptObject[] parameters)
        {
            var builder = _ObjectWithConstructorBuilder.Value;
            for (var i = 0; i < number; i++)
            {
                var args = new List<CefV8Value>()
                {
                    CefV8Value.CreateInt((int)_Count++),
                    constructor.Convert()
                };
                var command = builder.ExecuteFunction(null, args.Concat(parameters.Select(p => p.Convert())).ToArray());
                yield return UpdateObject(command);
            }
        }
    }
}
