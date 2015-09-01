using MVVM.HTML.Core.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

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
            T res = default(T);
            Action Compute = () => res = compute();
            _Dispatcher.Invoke(Compute);
            return res;
        }
    }
}
