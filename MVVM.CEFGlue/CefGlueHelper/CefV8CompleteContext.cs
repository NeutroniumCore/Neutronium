using CefGlue.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefGlueHelper
{
    public class CefV8CompleteContext : IDispatcher
    {
        public CefV8CompleteContext(CefV8Context iContext, CefTaskRunner iRunner)
        {
            Context = iContext;
            Runner = iRunner;
        }

        public CefV8Context Context { get; private set; }

        public CefTaskRunner Runner { get; private set; }

   

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
