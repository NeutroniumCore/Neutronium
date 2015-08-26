using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.Window
{
    public interface IDispatcher
    {
        Task RunAsync(Action act);

        void Run(Action act);

        Task<T> EvaluateAsync<T>(Func<T> compute);

        T Evaluate<T>(Func<T> compute);
    }
}
