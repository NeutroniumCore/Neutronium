using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.Awesomium.Internal;
using Awesomium_Core = Awesomium.Core;

namespace Neutronium.WebBrowserEngine.Awesomium.Engine
{
    internal class AwesomiumJavascriptObjectFactory : IJavascriptObjectFactory
    {
        private static readonly IDictionary<Type, Func<object, Awesomium_Core.IWebView, Awesomium_Core.JSValue>> _Converters =
         new Dictionary<Type, Func<object, Awesomium_Core.IWebView, Awesomium_Core.JSValue>>();

        private readonly Awesomium_Core.IWebView _IWebView;
        private IJavascriptObject _JSNull = null;

        public AwesomiumJavascriptObjectFactory(Awesomium_Core.IWebView iIWebView)
        {
            _IWebView = iIWebView;
        }

        private static void Register<T>(Func<T, Awesomium_Core.IWebView, Awesomium_Core.JSValue> Factory)
        {
            _Converters.Add(typeof(T), (o, b) => Factory((T)o,b));
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
            Register<decimal>((source, b) => new Awesomium_Core.JSValue((double)source));
            Register<bool>((source, b) => new Awesomium_Core.JSValue(source));
            Register<DateTime>((source, builder) => builder.EvaluateSafe(() =>
                        builder.ExecuteJavascriptWithResult(string.Format("new Date({0})",
                        string.Join(",", source.Year, source.Month - 1, source.Day, source.Hour, source.Minute, source.Second, source.Millisecond)))));
        }

        public bool Solve(object ifrom, out Awesomium_Core.JSValue res)
        {
            Func<object, Awesomium_Core.IWebView, Awesomium_Core.JSValue> conv = null;
            if (!_Converters.TryGetValue(ifrom.GetType(),out conv))
            {
                res = new Awesomium_Core.JSValue();
                return false;
            }

            res = conv(ifrom, _IWebView);
            return true;
        }

         public bool CreateBasic(object ifrom, out IJavascriptObject res)
         {
             res = null;
             Awesomium_Core.JSValue jsres;  
             bool bres = Solve(ifrom, out jsres);
             if (bres)
                 res = jsres.Convert();
             return bres;
         }

         public bool IsTypeBasic(Type itype)
         {
             if (itype == null)
                 return false;

             return _Converters.ContainsKey(itype);
         }

         private Awesomium_Core.JSValue Check(Awesomium_Core.JSObject ires)
         {
             if (ires == null)
                 throw ExceptionHelper.GetUnexpected();

             return ires;
         }

         private Awesomium_Core.JSValue UpdateObject(Awesomium_Core.JSObject ires)
         {
             ires["_MappedId"] = new Awesomium_Core.JSValue(_Count++);
             return ires;
         }

         public IJavascriptObject CreateNull()
         {
             if (_JSNull==null)
                 _JSNull = Check(_IWebView.EvaluateSafe(() => _IWebView.ExecuteJavascriptWithResult("new Null_reference()"))).Convert() ;

             return _JSNull;
         }

         private static uint _Count = 0;

         public IJavascriptObject CreateObject(bool iLocal)
         {
             string Name = string.Format("MVVM_HTML_{0}", _Count);
             return _IWebView.EvaluateSafe(() =>
                 {
                    Awesomium_Core.JSObject res = (iLocal) ? new Awesomium_Core.JSObject() :
                           (Awesomium_Core.JSObject)_IWebView.CreateGlobalJavascriptObject(Name);
                   
                     res["_MappedId"] = new Awesomium_Core.JSValue(_Count++);
                    
                     return res.Convert();
                 });
         }

         public IJavascriptObject CreateInt(int value)
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

         public IJavascriptObject CreateArray(IEnumerable<IJavascriptObject> iCount)
         {
             return new Awesomium_Core.JSValue(iCount.Select(o => o.Convert()).ToArray()).Convert();
         }

         public IJavascriptObject CreateObject(string iCreationCode)
         {
             return _IWebView.EvaluateSafe(() => UpdateObject(_IWebView.ExecuteJavascriptWithResult(iCreationCode))).Convert();
         }

         public IJavascriptObject CreateUndefined()
         {
             return Awesomium_Core.JSValue.Undefined.Convert();
         }
    }
}
