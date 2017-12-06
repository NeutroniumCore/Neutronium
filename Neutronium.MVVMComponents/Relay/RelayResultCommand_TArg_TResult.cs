using System;
using System.Threading.Tasks;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelayResultCommand<TArg, TResult> : IResultCommand<TArg, TResult>
    {
        private readonly Func<TArg, Task<TResult>> _ResultBuilder;
        public RelayResultCommand(Func<TArg, TResult> function)
        {
            _ResultBuilder = (arg) =>
            {
                var tcs = new TaskCompletionSource<TResult>();
                try
                {
                    tcs.SetResult(function(arg));
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }
                return tcs.Task;
            };
        }

        public RelayResultCommand(Func<TArg, Task<TResult>> resultBuilder)
        {
            _ResultBuilder = resultBuilder;
        }

        public async Task<TResult> Execute(TArg argument)
        {
            return await _ResultBuilder(argument);
        }
    }
}
