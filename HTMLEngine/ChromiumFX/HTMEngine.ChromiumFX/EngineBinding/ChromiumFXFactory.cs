using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chromium;
using Chromium.Remote;
using HTMEngine.ChromiumFX.Convertion;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace HTMEngine.ChromiumFX.EngineBinding 
{
    internal class ChromiumFXFactory : IJavascriptObjectFactory 
    {
        private static UInt32 _Count = 0;
        private readonly CfrV8Context _CfrV8Context;
        private static readonly IDictionary<Type, Func<object, CfrV8Value>> _Converters = new Dictionary<Type, Func<object, CfrV8Value>>();

        internal ChromiumFXFactory(CfrV8Context context) 
        {
            _CfrV8Context = context;
        }

        static ChromiumFXFactory() 
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

            res = conv(ifrom).Convert();
            return true;
        }

        public bool IsTypeBasic(Type itype)
        {
            if (itype == null)
                return false;

            return _Converters.ContainsKey(itype);
        }

        public IJavascriptObject CreateNull() 
        {
            return CfrV8Value.CreateNull().Convert();
        }

        private int _GlobalCount = 0;
        public IJavascriptObject CreateObject(bool iLocal) 
        {
            var rawResult = CfrV8Value.CreateObject(null);
            if (iLocal)
                return UpdateConvert(rawResult);

            _CfrV8Context.Global.SetValue(string.Format("__ChromiumFX_Object_{0}__", _GlobalCount++), rawResult, CfxV8PropertyAttribute.DontDelete | CfxV8PropertyAttribute.ReadOnly);
            return UpdateConvert(rawResult);
        }

        public IJavascriptObject CreateObject(string iCreationCode) 
        {
            CfrV8Value v8Res;
            CfrV8Exception exception;
            return (_CfrV8Context.Eval(iCreationCode, out v8Res, out exception)) ? UpdateConvert(v8Res) : null;
        }

        public IJavascriptObject CreateUndefined() 
        {
            return CfrV8Value.CreateUndefined().Convert();
        }

        public IJavascriptObject CreateInt(int value) 
        {
            return CfrV8Value.CreateInt(value).Convert();
        }

        public IJavascriptObject CreateDouble(double value) 
        {
            return CfrV8Value.CreateDouble(value).Convert();
        }

        public IJavascriptObject CreateString(string value) 
        {
            return CfrV8Value.CreateString(value).Convert();
        }

        public IJavascriptObject CreateBool(bool value) 
        {
            return CfrV8Value.CreateBool(value).Convert();
        }

        public IJavascriptObject CreateArray(IEnumerable<IJavascriptObject> iCollection) 
        {
            var col = iCollection.ToList();
            var res = CfrV8Value.CreateArray(col.Count);
            col.ForEach((el, i) => res.SetValue(i, el.Convert()));
            return UpdateConvert(res);
        }

        private IJavascriptObject UpdateConvert(CfrV8Value value) 
        {
            if (value == null)
                return null;

            value.SetValue("_MappedId", CfrV8Value.CreateUint(_Count++), CfxV8PropertyAttribute.DontDelete  | CfxV8PropertyAttribute.DontEnum
                        |  CfxV8PropertyAttribute.ReadOnly);

            return value.Convert();
        }
    }
}
