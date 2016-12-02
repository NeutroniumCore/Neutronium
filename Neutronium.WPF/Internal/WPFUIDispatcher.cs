using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.WPF.Internal
{
    public class WPFUIDispatcher : IDispatcher
    {
        private Dispatcher _Dispatcher;
        public WPFUIDispatcher(Dispatcher iDispatcher)
        {
            _Dispatcher = iDispatcher;
        }

        public Task RunAsync(Action act)
        {
            var tcs = new TaskCompletionSource<object>();
            Action doact = () =>
            {
                act();
                tcs.SetResult(null);
            };
            BeginInvoke(doact);
            return tcs.Task;
        }

        public void Run(Action act)
        {
            Invoke(act);
        }

        public Task<T> EvaluateAsync<T>(Func<T> compute)
        {
            var tcs = new TaskCompletionSource<T>();
            Action doact = () =>
            {
                tcs.SetResult(compute());
            };
            BeginInvoke(doact);
            return tcs.Task;
        }

        public T Evaluate<T>(Func<T> compute)
        {
            var res = default(T);
            Action action = () => res = compute();
            Invoke(action);
            return res;
        }

        private void Invoke(Action action)
        {
            if (_Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _Dispatcher.Invoke(action);
            }
        }

        private void BeginInvoke(Action action)
        {
            if (_Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _Dispatcher.BeginInvoke(action);
            }
        }

        public bool IsInContext() 
        {
            return _Dispatcher.Thread == Thread.CurrentThread;
        }
    }
}
