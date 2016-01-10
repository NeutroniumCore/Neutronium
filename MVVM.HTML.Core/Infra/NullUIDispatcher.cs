using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.HTML.Core.Window;

namespace MVVM.HTML.Core.Infra
{
    public class NullUIDispatcher : IDispatcher
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
            return Task.FromResult<T>(compute());
        }

        public T Evaluate<T>(Func<T> compute)
        {
            return compute();
        }
    }
}
