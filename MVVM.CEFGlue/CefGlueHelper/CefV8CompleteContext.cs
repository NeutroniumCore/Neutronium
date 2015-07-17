using CefGlue.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefGlueHelper
{
    public class CefV8CompleteContext : IUIDispatcher
    {
        public CefV8CompleteContext(CefV8Context iContext, CefTaskRunner iRunner)
        {
            Context = iContext;
            Runner = iRunner;
        }

        public CefV8Context Context { get; private set; }

        public CefTaskRunner Runner { get; private set; }


        public Task RunInContextAsync(Action ac)
        {
            Action InContext = () =>
                {
                    Context.Enter();
                    ac();
                    Context.Exit();
                };
            return Runner.RunInContextAsync(InContext);
        }

     

        public Task RunAsync(Action act)
        {
            Action InContext = () =>
            {
                Context.Enter();
                act();
                Context.Exit();
            };
            return Runner.RunInContextAsync(InContext);
        }

        public Task DispatchAsync(Action act)
        {
            Action InContext = () =>
            {
                Context.Enter();
                act();
                Context.Exit();
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
                Context.Enter();
                T res = compute();
                Context.Exit();
                return res;
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


        public IDisposable Enter()
        {
            return new ContextOpener(Context);
        }
    }
}
