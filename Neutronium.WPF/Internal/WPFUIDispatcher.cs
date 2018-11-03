using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.WPF.Internal
{
    public class WPFUIDispatcher : IDispatcher
    {
        private readonly Dispatcher _Dispatcher;
        public WPFUIDispatcher(Dispatcher iDispatcher)
        {
            _Dispatcher = iDispatcher;
        }

        public Task RunAsync(Action act)
        {
            var tcs = new TaskCompletionSource<object>();
            void Act()
            {
                act();
                tcs.SetResult(null);
            }
            BeginInvoke(Act);
            return tcs.Task;
        }

        public void Dispatch(Action act)
        {
            BeginInvoke(act);
        }

        public void Run(Action act)
        {
            Invoke(act);
        }

        public Task<T> EvaluateAsync<T>(Func<T> compute)
        {
            var tcs = new TaskCompletionSource<T>();
            void Action() => tcs.SetResult(compute());
            BeginInvoke(Action);
            return tcs.Task;
        }

        public T Evaluate<T>(Func<T> compute)
        {
            var res = default(T);
            void Action() => res = compute();
            Invoke(Action);
            return res;
        }

        private void Invoke(Action action)
        {
            DoSynchroneIfPossible(action, (d, act) => d.Invoke(act));
        }

        private void BeginInvoke(Action action)
        {
            DoSynchroneIfPossible(action, (d, act) => d.BeginInvoke(act, DispatcherPriority.Send));
        }

        private void DoSynchroneIfPossible(Action action, Action<Dispatcher, Action> doAsync)
        {
            if (IsInContext())
            {
                action();
            }
            else
            {
                doAsync(_Dispatcher, action);
            }
        }

        public bool IsInContext()
        {
            return _Dispatcher.CheckAccess();
        }
    }
}
