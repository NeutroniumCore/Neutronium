using CefGlue.Window;
using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefGlueHelper
{
    public class CefV8CompleteContext : IWebView 
    {
        public CefV8CompleteContext(CefV8Context iContext, CefTaskRunner iRunner)
        {
            Context = iContext;
            Runner = iRunner;
        }

        public CefV8Value GetGlobal() 
        {
            return Context.GetGlobal();
        }

        public bool Eval(string code, out CefV8Value res )
        {
            CefV8Exception exp=null;
            return Context.TryEval(code, out res, out exp);
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

        public Task DispatchAsync(Action act)
        {
            Action InContext = () =>
            {
                using (Enter())
                {
                    act();
                }
            };
            return Runner.DispatchAsync(InContext);
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

        private class ContextOpener : IDisposable
        {
            private CefV8Context _CefV8Context;
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
    }
}
