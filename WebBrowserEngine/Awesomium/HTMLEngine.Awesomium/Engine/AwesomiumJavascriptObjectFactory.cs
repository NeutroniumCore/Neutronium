using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.Awesomium.Internal;
using Awesomium_Core = Awesomium.Core;

namespace Neutronium.WebBrowserEngine.Awesomium.Engine
{
    internal class AwesomiumJavascriptObjectFactory : IJavascriptObjectFactory
    {
        private static readonly IDictionary<Type, Func<object, Awesomium_Core.IWebView, Awesomium_Core.JSValue>>
            _Converters =
                new Dictionary<Type, Func<object, Awesomium_Core.IWebView, Awesomium_Core.JSValue>>();

        private readonly Awesomium_Core.IWebView _IWebView;
        private IJavascriptObject _JsNull = null;

        public AwesomiumJavascriptObjectFactory(Awesomium_Core.IWebView iIWebView)
        {
            _IWebView = iIWebView;
        }

        private static void Register<T>(Func<T, Awesomium_Core.IWebView, Awesomium_Core.JSValue> Factory)
        {
            _Converters.Add(typeof(T), (o, b) => Factory((T) o, b));
        }

        public Type GetElementType(IEnumerable collection)
        {
            var typeo = collection.GetType();
            var elementtype = typeo.GetEnumerableBase();
            if (elementtype == null)
                return null;
            var almost = elementtype.GetUnderlyingType();
            return _Converters.ContainsKey(almost) ? almost : null;
        }

        static AwesomiumJavascriptObjectFactory()
        {
            Register<string>((source, b) => new Awesomium_Core.JSValue(source));
            Register<Int64>((source, b) => new Awesomium_Core.JSValue(source));
            Register<Int32>((source, b) => new Awesomium_Core.JSValue(source));
            Register<Int16>((source, b) => new Awesomium_Core.JSValue(source));
            Register<UInt64>((source, b) => new Awesomium_Core.JSValue(source));
            Register<UInt32>((source, b) => new Awesomium_Core.JSValue(source));
            Register<UInt16>((source, b) => new Awesomium_Core.JSValue(source));
            Register<float>((source, b) => new Awesomium_Core.JSValue(source));
            Register<char>((source, b) => new Awesomium_Core.JSValue(source));
            Register<double>((source, b) => new Awesomium_Core.JSValue(source));
            Register<decimal>((source, b) => new Awesomium_Core.JSValue((double) source));
            Register<bool>((source, b) => new Awesomium_Core.JSValue(source));
            Register<DateTime>((source, builder) => builder.EvaluateSafe(() =>
                builder.ExecuteJavascriptWithResult(
                    $"new Date({string.Join(",", source.Year, source.Month - 1, source.Day, source.Hour, source.Minute, source.Second, source.Millisecond)})")));
        }

        public bool Solve(object ifrom, out Awesomium_Core.JSValue res)
        {
            if (!_Converters.TryGetValue(ifrom.GetType(), out var conv))
            {
                res = new Awesomium_Core.JSValue();
                return false;
            }

            res = conv(ifrom, _IWebView);
            return true;
        }

        public static bool IsTypeConvertible(Type type)
        {
            return type != null && _Converters.ContainsKey(type);
        }

        private Awesomium_Core.JSValue Check(Awesomium_Core.JSObject ires)
        {
            if (ires == null)
                throw ExceptionHelper.GetUnexpected();

            return ires;
        }

        private Awesomium_Core.JSValue UpdateObject(Awesomium_Core.JSObject ires)
        {
            ires[NeutroniumConstants.ObjectId] = new Awesomium_Core.JSValue(_Count++);
            return ires;
        }

        public IJavascriptObject CreateNull()
        {
            if (_JsNull == null)
                _JsNull = Check(
                        _IWebView.EvaluateSafe(() => _IWebView.ExecuteJavascriptWithResult("new Null_reference()")))
                    .Convert();

            return _JsNull;
        }

        private static uint _Count = 0;

        private IJavascriptObject CreateJSObject(bool local)
        {
            var Name = $"MVVM_HTML_{_Count}";
            return _IWebView.EvaluateSafe(() =>
            {
                Awesomium_Core.JSObject res = (local)
                    ? new Awesomium_Core.JSObject()
                    : (Awesomium_Core.JSObject) _IWebView.CreateGlobalJavascriptObject(Name);

                res[NeutroniumConstants.ObjectId] = new Awesomium_Core.JSValue(_Count++);

                return res.Convert();
            });
        }

        public IJavascriptObject CreateObject()
        {
            return CreateJSObject(false);
        }

        public IJavascriptObject CreateObject(ObjectObservability objectObservability)
        {
            return CreateJSObject(true);
        }

        public IEnumerable<IJavascriptObject> CreateObjects(int readWrite, int readOnlyNumber)
        {
            var count = readWrite + readOnlyNumber;
            for (var i = 0; i < count; i++)
            {
                yield return CreateObject( default(ObjectObservability));
            }
        }

        public IJavascriptObject CreateInt(int value)
        {
            return new Awesomium_Core.JSValue(value).Convert();
        }

        public IJavascriptObject CreateUint(uint value)
        {
            return new Awesomium_Core.JSValue(value).Convert();
        }

        public IJavascriptObject CreateDouble(double value)
        {
            return new Awesomium_Core.JSValue(value).Convert();
        }

        public IJavascriptObject CreateString(string value)
        {
            return new Awesomium_Core.JSValue(value).Convert();
        }

        public IJavascriptObject CreateBool(bool value)
        {
            return new Awesomium_Core.JSValue(value).Convert();
        }

        public IJavascriptObject CreateDateTime(DateTime source)
        {
            var result = _IWebView.EvaluateSafe(() =>
                _IWebView.ExecuteJavascriptWithResult(
                    $"new Date({string.Join(",", source.Year, source.Month - 1, source.Day, source.Hour, source.Minute, source.Second, source.Millisecond)})"));

            return result.Convert();
        }

        public IJavascriptObject CreateArray(IEnumerable<IJavascriptObject> count)
        {
            return new Awesomium_Core.JSValue(count.Select(o => o.Convert()).ToArray()).Convert();
        }

        public IEnumerable<IJavascriptObject> CreateArrays(int number)
        {
            for (var i = 0; i < number; i++)
            {
                yield return CreateArray(0);
            }
        }

        public IJavascriptObject CreateArray(int size)
        {
            return new Awesomium_Core.JSValue(Enumerable.Repeat(new Awesomium_Core.JSValue(), size).ToArray())
                .Convert();
        }

        public IJavascriptObject CreateObject(string iCreationCode)
        {
            return _IWebView.EvaluateSafe(() => UpdateObject(_IWebView.ExecuteJavascriptWithResult(iCreationCode)))
                .Convert();
        }

        public IJavascriptObject CreateUndefined()
        {
            return Awesomium_Core.JSValue.Undefined.Convert();
        }

        public IEnumerable<IJavascriptObject> CreateObjectsFromContructor(int number, IJavascriptObject constructor,
            params IJavascriptObject[] parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IJavascriptObject> CreateFromExcecutionCode(IEnumerable<string> from)
        {
            foreach (var code in from)
            {
                yield return CreateObject(code);
            }
        }
    }
}
