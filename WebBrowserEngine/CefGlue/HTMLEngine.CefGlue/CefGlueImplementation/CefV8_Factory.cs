using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Xilium.CefGlue;
using MoreCollection.Extensions;

namespace Neutronium.WebBrowserEngine.CefGlue.CefGlueImplementation
{
    internal class CefV8_Factory : IJavascriptObjectFactory
    {
        private static uint _Count = 0;

        private static readonly IDictionary<Type, Func<object, CefV8Value>> _Converters = new Dictionary<Type, Func<object, CefV8Value>>();
        private readonly IWebView _CefV8_WebView;

        static CefV8_Factory()
        {
            Register<string>(CefV8Value.CreateString);

            Register<Int64>((source) => CefV8Value.CreateDouble((double)source));
            Register<UInt64>((source) => CefV8Value.CreateDouble((double)source));
            Register<float>((source) => CefV8Value.CreateDouble((double)source));

            Register<Int32>(CefV8Value.CreateInt);
            Register<Int16>((source) => CefV8Value.CreateInt((int)source));

            Register<UInt32>(CefV8Value.CreateUInt);
            Register<UInt16>((source) => CefV8Value.CreateUInt((UInt32)source));

            //check two way and convertion back
            Register<char>((source) => CefV8Value.CreateString(new StringBuilder().Append(source).ToString()));
            Register<double>(CefV8Value.CreateDouble);
            Register<decimal>((source) => CefV8Value.CreateDouble((double)source));
            Register<bool>(CefV8Value.CreateBool);
            Register<DateTime>(CefV8Value.CreateDate);
        }

        private readonly Lazy<CefV8Value> _ObjectWithConstructorBuilder;

        public CefV8_Factory(IWebView iCefV8_WebView)
        {
            _CefV8_WebView = iCefV8_WebView;
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
            _CefV8_WebView.Eval(code, out res);
            return res.Convert();
        }

        private static void Register<T>(Func<T, CefV8Value> Factory)
        {
            _Converters.Add(typeof(T), (o) => Factory((T)o));
        }

        public bool CreateBasic(object ifrom, out IJavascriptObject res)
        {
            Func<object, CefV8Value> conv;
            if (!_Converters.TryGetValue(ifrom.GetType(), out conv))
            {
                res = null;
                return false;
            }

            res = new CefV8_JavascriptObject( _CefV8_WebView.Evaluate(() => conv(ifrom)) );
            return true;
        }

        public IEnumerable<IJavascriptObject> CreateBasics(IEnumerable<object> from)
        {
            foreach (var @object in from)
            {
                IJavascriptObject res = null;
                yield return CreateBasic(@object, out res) ? res : null;
            }
        }

        public static bool IsTypeConvertible(Type itype) 
        {
            return itype != null && _Converters.ContainsKey(itype);
        }

        public bool IsTypeBasic(Type type) 
        {
            return IsTypeConvertible(type);
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

        public IJavascriptObject CreateString(string value)
        {
            return new CefV8_JavascriptObject(CefV8Value.CreateString(value));
        }

        public IJavascriptObject CreateBool(bool value)
        {
            return new CefV8_JavascriptObject(CefV8Value.CreateBool(value));
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
            return _CefV8_WebView.Evaluate(() =>
            {
                IJavascriptObject res;

                _CefV8_WebView.Eval(iCreationCode, out res);

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
