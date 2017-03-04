using System;
using System.Threading.Tasks;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelayResultCommand<TIn, TResult> : IResultCommand
    {
        private readonly Func<TIn, Task<TResult>> _ResultBuilder;
        public RelayResultCommand(Func<TIn, TResult> function)
        {
            _ResultBuilder = (iargument) =>
                {
                    var tcs = new TaskCompletionSource<TResult>();
                    try
                    {
                        tcs.SetResult(function(iargument));
                    }
                    catch(Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                    return tcs.Task;
                };
        }

        public RelayResultCommand(Func<TIn, Task<TResult>> resultBuilder)
        {
            _ResultBuilder = resultBuilder;
        }

        public async Task<object> Execute(object argument)
        {
            return await _ResultBuilder((TIn)argument);
        }
    }
}
