using System;
using System.Threading.Tasks;

namespace MVVM.Component
{
    public class RelayResultCommand<TResult> : IResultCommand
    {
        private readonly Func<TResult> _Function;
        public RelayResultCommand(Func<TResult> iFunction)
        {
            _Function = iFunction;
        }
        public Task<object> Execute(object iargument)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(_Function());
            return tcs.Task;
        }
    }
}
