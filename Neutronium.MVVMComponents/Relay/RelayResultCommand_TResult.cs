using System;
using System.Threading.Tasks;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelayResultCommand<TResult> : IResultCommand<TResult>
    {
        private readonly Func<TResult> _Function;
        public RelayResultCommand(Func<TResult> function)
        {
            _Function = function;
        }

        public Task<TResult> Execute()
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(_Function());
            return tcs.Task;
        }
    }
}
