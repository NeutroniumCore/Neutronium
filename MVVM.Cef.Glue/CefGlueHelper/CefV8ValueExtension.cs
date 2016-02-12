using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.CefGlueHelper
{
    public static class CefV8ValueExtension
    {
        private  static IEnumerable<CefV8Value> GetElements(this CefV8Value @this)
        {
            if (!@this.IsArray)
                throw new ArgumentException("Method valid only for array");

            int count = @this.GetArrayLength();

            for(int i=0; i< count; i++)
            {
                yield return @this.GetValue(i);
            }
        }

        public static CefV8Value[] GetArrayElements(this CefV8Value @this)
        {
            return @this.GetElements().ToArray();     
        }

        public static CefV8Value Invoke(this CefV8Value @this, string functionname, IWebView iCefV8Context, params CefV8Value[] args)
        {
            return @this.InvokeAsync(functionname, iCefV8Context, args).Result;
        }

        public static CefV8Value ExecuteFunction(this CefV8Value @this)
        {
            return @this.ExecuteFunction(null, new CefV8Value[] { });
        }

        public static Task<CefV8Value> InvokeAsync(this CefV8Value @this, string functionname, IWebView context, params CefV8Value[] args)
        {
            
            return context.EvaluateAsync(() =>
                {
                    var fn = @this.GetValue(functionname);
                    if ((fn==null) || !fn.IsFunction)
                        return CefV8Value.CreateUndefined();
                    return fn.ExecuteFunction(@this, args);
                }
                );
        }

        public static void Bind(this CefV8Value @this, string functionname, IWebView iCefV8Context, Action<string, CefV8Value, CefV8Value[]> iaction)
        {
            iCefV8Context.RunAsync(() =>
                {
                    var function = CefV8Value.CreateFunction(functionname, new CefV8Handler_Action(iaction));
                    @this.SetValue(functionname, function, CefV8PropertyAttribute.None);
                });
        }
    }
}
