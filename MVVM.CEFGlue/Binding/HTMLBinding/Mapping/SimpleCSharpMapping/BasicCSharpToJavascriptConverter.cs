using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xilium.CefGlue;

using MVVM.CEFGlue.Infra;
using MVVM.CEFGlue.CefGlueHelper;
using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;


namespace MVVM.CEFGlue.HTMLBinding
{
    internal class BasicCSharpToJavascriptConverter
    {
        private IWebView _CefV8Context;
        internal BasicCSharpToJavascriptConverter(IWebView iContext)
        {
            _CefV8Context = iContext;
        }

        private static IDictionary<Type, Func<object, CefV8Value>> _Converters = new Dictionary<Type, Func<object, CefV8Value>>();
        private static void Register<T>(Func<T, CefV8Value> Factory)
        {
            _Converters.Add(typeof(T), (o) => Factory((T)o));
        }

        public Type GetElementType(IEnumerable collection)
        {
            var typeo = collection.GetType();
            var elementtype = typeo.GetEnumerableBase();
            if (elementtype == null)
                return null;
            var almost = elementtype.GetUnderlyingNullableType() ?? elementtype;
            return _Converters.ContainsKey(almost) ? almost : null;
        }

        static BasicCSharpToJavascriptConverter()
        {
            Register<string>((source) => CefV8Value.CreateString(source));

            Register<Int64>((source) => CefV8Value.CreateDouble((double)source));
            Register<UInt64>((source) =>  CefV8Value.CreateDouble((double)source));
            Register<float>((source) => CefV8Value.CreateDouble((double)source));

            Register<Int32>((source) => CefV8Value.CreateInt(source));
            Register<Int16>((source) => CefV8Value.CreateInt((int)source));

            Register<UInt32>((source) => CefV8Value.CreateUInt(source));
            Register<UInt16>((source) => CefV8Value.CreateUInt((UInt32)source));

            //check two way and convertion back
            Register<char>((source) => CefV8Value.CreateString( new StringBuilder().Append(source).ToString()));

            Register<double>((source) => CefV8Value.CreateDouble(source));
            Register<decimal>((source) => CefV8Value.CreateDouble((double)source));
            Register<bool>((source) => CefV8Value.CreateBool(source));
            Register<DateTime>((source) => CefV8Value.CreateDate(source));
        }

        public bool Solve(object ifrom, out CefV8Value res)
        {
            Func<object, CefV8Value> conv = null;
            if (!_Converters.TryGetValue(ifrom.GetType(),out conv))
            {
                res = null;
                return false;
            }

          
            res = _CefV8Context.Evaluate(() => conv(ifrom));
                //{  
                //    //_CefV8Context.Enter();
                //    CefV8Value myres = conv(ifrom); 
                //    //_CefV8Context.Exit();
                //    return myres;

                //}).Result;

           
            return true;
        }
    }
}
