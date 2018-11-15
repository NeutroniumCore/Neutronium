using System;
using System.Threading.Tasks;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Infra
{
    public class NullUIDispatcher : IUiDispatcher
    {
        public Task RunAsync(Action act)
        {
            act();
            return TaskHelper.Ended();
        }

        public void Dispatch(Action act)
        {
            act();
        }

        public void Run(Action act)
        {
            act();
        }

        public Task<T> EvaluateAsync<T>(Func<T> compute)
        {
            return Task.FromResult(compute());
        }

        public T Evaluate<T>(Func<T> compute)
        {
            return compute();
        }

        public bool IsInContext() 
        {
            return true;
        }

        public void DispatchWithBindingPriority(Action act)
        {
            act();
        }
    }
}
