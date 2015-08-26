using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xilium.CefGlue;

using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Exceptions;

using MVVM.Cef.Glue.CefGlueHelper;


namespace MVVM.Cef.Glue
{
    internal class CefV8_Factory : IJavascriptObjectFactory
    {
        private static uint _Count = 0;

        private static IDictionary<Type, Func<object, CefV8Value>> _Converters = new Dictionary<Type, Func<object, CefV8Value>>();


        static CefV8_Factory()
        {
            Register<string>((source) => CefV8Value.CreateString(source));

            Register<Int64>((source) => CefV8Value.CreateDouble((double)source));
            Register<UInt64>((source) => CefV8Value.CreateDouble((double)source));
            Register<float>((source) => CefV8Value.CreateDouble((double)source));

            Register<Int32>((source) => CefV8Value.CreateInt(source));
            Register<Int16>((source) => CefV8Value.CreateInt((int)source));

            Register<UInt32>((source) => CefV8Value.CreateUInt(source));
            Register<UInt16>((source) => CefV8Value.CreateUInt((UInt32)source));

            //check two way and convertion back
            Register<char>((source) => CefV8Value.CreateString(new StringBuilder().Append(source).ToString()));

            Register<double>((source) => CefV8Value.CreateDouble(source));
            Register<decimal>((source) => CefV8Value.CreateDouble((double)source));
            Register<bool>((source) => CefV8Value.CreateBool(source));
            Register<DateTime>((source) => CefV8Value.CreateDate(source));
        }

        
        private static void Register<T>(Func<T, CefV8Value> Factory)
        {
            _Converters.Add(typeof(T), (o) => Factory((T)o));
        }


        private IWebView _CefV8_WebView;
        public CefV8_Factory(IWebView iCefV8_WebView)
        {
            _CefV8_WebView = iCefV8_WebView;
        }

        public bool SolveBasic(object ifrom, out IJavascriptObject res)
        {
            Func<object, CefV8Value> conv = null;
            if (!_Converters.TryGetValue(ifrom.GetType(), out conv))
            {
                res = null;
                return false;
            }

            res = new CefV8_JavascriptObject( _CefV8_WebView.Evaluate(() => conv(ifrom)) );
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
            return new CefV8_JavascriptObject(CefV8Value.CreateNull());
        }

        public IJavascriptObject CreateObject()
        {
            return UpdateObject(CefV8Value.CreateObject(null));
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

        public IJavascriptObject CreateArray(int iCount)
        {
            return UpdateObject(CefV8Value.CreateArray(iCount));
        }

        private void BasicUpdateObject(CefV8Value ires)
        {
            if (ires != null)
            {
                ires.SetValue("_MappedId", CefV8Value.CreateUInt(_Count++),
                    CefV8PropertyAttribute.ReadOnly | CefV8PropertyAttribute.DontEnum | CefV8PropertyAttribute.DontDelete);
            }
        }

        private CefV8_JavascriptObject UpdateObject(CefV8_JavascriptObject ires)
        {
            BasicUpdateObject(ires.RawValue);
            return ires;
        }

        private CefV8_JavascriptObject UpdateObject(CefV8Value ires)
        {
            BasicUpdateObject(ires);
            return new CefV8_JavascriptObject(ires);
        }

        private CefV8_JavascriptObject Check(CefV8_JavascriptObject ires)
        {
            if (ires == null)
                throw ExceptionHelper.NoKoExtension();

            return ires;
        }

        private CefV8_JavascriptObject CheckUpdate(CefV8_JavascriptObject ires)
        {
            return UpdateObject(Check(ires));
        }

        public IJavascriptObject CreateEnum(Enum ienum)
        {
            return _CefV8_WebView.Evaluate(() =>
                {
                    IJavascriptObject res = null;

                    _CefV8_WebView.Eval(string.Format("new Enum('{0}',{1},'{2}','{3}')",
                        ienum.GetType().Name, Convert.ToInt32(ienum), ienum.ToString(), ienum.GetDescription()),
                        out res);

                    return CheckUpdate(res as CefV8_JavascriptObject);
                });
        }
    }
}
