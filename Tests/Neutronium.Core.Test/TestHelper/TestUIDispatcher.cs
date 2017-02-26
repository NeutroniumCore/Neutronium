using Neutronium.Core.WebBrowserEngine.Window;
using System;
using System.Threading.Tasks;

namespace Neutronium.Core.Test.TestHelper
{
    public class TestUIDispatcher : IDispatcher
    {
        public Task RunAsync(Action act)
        {
            act();
            return Task.FromResult<object>(null);
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
    }
}
