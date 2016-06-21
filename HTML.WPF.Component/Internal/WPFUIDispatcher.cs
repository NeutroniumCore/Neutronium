using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace HTML_WPF.Component
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
            _Dispatcher.BeginInvoke(doact);
            return tcs.Task;
        }

        public void Run(Action act)
        {
            _Dispatcher.Invoke(act);
        }

        public Task<T> EvaluateAsync<T>(Func<T> compute)
        {
            var tcs = new TaskCompletionSource<T>();
            Action doact = () =>
            {
                tcs.SetResult(compute());
            };
            _Dispatcher.BeginInvoke(doact);
            return tcs.Task;
        }

        public T Evaluate<T>(Func<T> compute)
        {
            var res = default(T);
            Action action = () => res = compute();
            _Dispatcher.Invoke(action);
            return res;
        }

        public bool IsInContext() 
        {
            return _Dispatcher.Thread == Thread.CurrentThread;
        }
    }
}
