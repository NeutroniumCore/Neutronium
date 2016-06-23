using System;
using System.Threading.Tasks;

namespace MVVM.Component.Relay
{
    public class RelayResultCommand<TIn, TResult> : IResultCommand
    {
        private readonly Func<TIn, Task<TResult>> _ResultBuilder;
        public RelayResultCommand(Func<TIn, TResult> iFunction)
        {
            _ResultBuilder = (iargument) =>
                {
                    var tcs = new TaskCompletionSource<TResult>();
                    try
                    {
                        tcs.SetResult(iFunction(iargument));
                    }
                    catch(Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                    return tcs.Task;
                };
        }

        public RelayResultCommand(Func<TIn, Task<TResult>> iResultBuilder)
        {
            _ResultBuilder = iResultBuilder;
        }

        public async Task<object> Execute(object iargument)
        {
            return await _ResultBuilder((TIn)iargument);
        }
    }
}
