using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chromium;
using Chromium.Remote;
using MoreCollection.Extensions;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.Convertion;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding 
{
    internal class ChromiumFxFactory : IJavascriptObjectFactory 
    {
        private static UInt32 _Count = 0;
        private readonly CfrV8Context _CfrV8Context;
        private static readonly IDictionary<Type, Func<object, CfrV8Value>> _Converters = new Dictionary<Type, Func<object, CfrV8Value>>();
        private IJavascriptObject _Null;

        internal ChromiumFxFactory(CfrV8Context context) 
        {
            _CfrV8Context = context;
        }

        static ChromiumFxFactory() 
        {
            Register<string>(CfrV8Value.CreateString);
            Register<Int64>((source) => CfrV8Value.CreateDouble((double) source));
            Register<UInt64>((source) => CfrV8Value.CreateDouble((double) source));
            Register<float>((source) => CfrV8Value.CreateDouble((double) source));
            Register<Int32>(CfrV8Value.CreateInt);
            Register<Int16>((source) => CfrV8Value.CreateInt((int) source));
            Register<UInt32>(CfrV8Value.CreateUint);
            Register<UInt16>((source) => CfrV8Value.CreateUint((UInt32) source));
            Register<char>((source) => CfrV8Value.CreateString(new StringBuilder().Append(source).ToString()));
            Register<double>(CfrV8Value.CreateDouble);
            Register<decimal>((source) => CfrV8Value.CreateDouble((double) source));
            Register<bool>(CfrV8Value.CreateBool);
            Register<DateTime>((source) => CfrV8Value.CreateDate(CfrTime.FromUniversalTime(source.ToUniversalTime())));
        }

        public static bool IsTypeConvertible(Type itype) 
        {
            return itype != null && _Converters.ContainsKey(itype);
        }

        private static void Register<T>(Func<T, CfrV8Value> Factory) 
        {
            _Converters.Add(typeof(T), (o) => Factory((T) o));
        }

        public bool CreateBasic(object ifrom, out IJavascriptObject res) 
        {
            Func<object, CfrV8Value> conv;
            if (!_Converters.TryGetValue(ifrom.GetType(), out conv)) 
            {
                res = null;
                return false;
            }

            res = conv(ifrom).ConvertBasic();
            return true;
        }

        public bool IsTypeBasic(Type type) 
        {
            return IsTypeConvertible(type);
        }

        public IJavascriptObject CreateNull() 
        {
            return _Null?? (_Null = CfrV8Value.CreateNull().ConvertBasic());
        }

        public IJavascriptObject CreateObject(bool local) 
        {
            var res = CfrV8Value.CreateObject(null, null);
            return UpdateConvert(res, false);
        }

        public IJavascriptObject CreateObject(string creationCode) 
        {
            var v8Res = Eval(creationCode);
            return (v8Res!=null) ? UpdateConvert(v8Res) : null;
        }

        public IJavascriptObject CreateUndefined() 
        {
            return CfrV8Value.CreateUndefined().ConvertBasic();
        }

        public IJavascriptObject CreateInt(int value) 
        {
            return CfrV8Value.CreateInt(value).ConvertBasic();
        }

        public IJavascriptObject CreateDouble(double value) 
        {
            return CfrV8Value.CreateDouble(value).ConvertBasic();
        }

        public IJavascriptObject CreateString(string value) 
        {
            return CfrV8Value.CreateString(value).ConvertBasic();
        }

        public IJavascriptObject CreateBool(bool value) 
        {
            return CfrV8Value.CreateBool(value).ConvertBasic();
        }

        public IJavascriptObject CreateArray(IEnumerable<IJavascriptObject> collection) 
        {
            var col = collection.ToList();
            var res = CfrV8Value.CreateArray(col.Count);
            col.ForEach((el, i) => res.SetValue(i, el.Convert()));
            return UpdateConvert(res, true);
        }

        public IJavascriptObject CreateArray(int size)
        {
            var res = CfrV8Value.CreateArray(0);
            return UpdateConvert(res, true);
        }

        private CfrV8Value Eval(string code)
        {
            CfrV8Value v8Res;
            CfrV8Exception exception;
            _CfrV8Context.Eval(code, string.Empty, 1, out v8Res, out exception);
            return v8Res;
        }

        private IJavascriptObject UpdateConvert(CfrV8Value value, bool isArray=false) 
        {
            if (value == null)
                return null;

            var id = _Count++;
            value.SetValue("_MappedId", CfrV8Value.CreateUint(id), CfxV8PropertyAttribute.DontDelete  | CfxV8PropertyAttribute.DontEnum
                        |  CfxV8PropertyAttribute.ReadOnly);

            return isArray? value.ConvertBasic(id) : value.ConvertObject(id);
        }
    }
}
