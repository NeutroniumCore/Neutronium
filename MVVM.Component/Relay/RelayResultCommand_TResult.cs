using System;
using System.Threading.Tasks;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelayResultCommand<TResult> : IResultCommand
    {
        private readonly Func<TResult> _Function;
        public RelayResultCommand(Func<TResult> function)
        {
            _Function = function;
        }

        public Task<object> Execute(object iargument)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(_Function());
            return tcs.Task;
        }
    }
}
