using System;
using System.Threading.Tasks;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.CefGlue.CefGlueHelper;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.CefGlueImplementation
{
    public class CefV8_WebView : IWebView 
    {
        private CefV8_Converter _CefV8_Converter;
        private CefV8_Factory _CefV8_Factory;

        public CefV8_WebView(CefV8Context iContext, CefTaskRunner iRunner)
        {
            Context = iContext;
            Runner = iRunner;
            _CefV8_Converter = new CefV8_Converter();
            _CefV8_Factory = new CefV8_Factory(this);
        }

        public IJavascriptObject GetGlobal() 
        {
            return new CefV8_JavascriptObject( Context.GetGlobal() );
        }

        public bool Eval(string code, out IJavascriptObject res)
        {
            CefV8Exception exp=null;
            CefV8Value val = null;
            bool bres = Context.TryEval(code, out val, out exp);
            res = (val == null) ? null : new CefV8_JavascriptObject(val);
            return bres;
        } 

        private CefV8Context Context { get; set; }

        private CefTaskRunner Runner { get; set; }

        public Task RunAsync(Action act)
        {
            Action InContext = () =>
            {
                using (Enter())
                {
                    act();
                }
            };
            return Runner.RunAsync(InContext);
        }

        public void Run(Action act)
        {
            RunAsync(act).Wait();
        }

        public Task<T> EvaluateAsync<T>(Func<T> compute)
        {
            Func<T> computeInContext = () =>
            {
                using (Enter())
                {
                    return compute();
                }
            };

            return Runner.EvaluateAsync(computeInContext);
        }

        public  T Evaluate<T>(Func<T> compute)
        {
            return EvaluateAsync(compute).Result;
        }

        public bool IsInContext()
        {
            return Runner.BelongsToCurrentThread;
        }

        private class ContextOpener : IDisposable
        {
            private readonly CefV8Context _CefV8Context;
            internal ContextOpener(CefV8Context iCefV8Context)
            {
                _CefV8Context = iCefV8Context;
                _CefV8Context.Enter();
            }

            void IDisposable.Dispose()
            {
                _CefV8Context.Exit();
            }
        }

        private IDisposable Enter()
        {
            return new ContextOpener(Context);
        }


        public IJavascriptObjectFactory Factory
        {
            get { return _CefV8_Factory; }
        }

        public IJavascriptObjectConverter Converter
        {
            get { return _CefV8_Converter; }
        }

        public void ExecuteJavaScript(string code)
        {
            RunAsync(() =>
                {
                    IJavascriptObject res;
                    Eval(code, out res);
                });
        }
    }
}
