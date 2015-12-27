using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Component
{
    public class RelayResultCommand<Tin, TResult> : IResultCommand
    {
        private Func<Tin, Task<TResult>> _ResultBuilder;
        public RelayResultCommand(Func<Tin, TResult> iFunction)
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

        public RelayResultCommand(Func<Tin, Task<TResult>> iResultBuilder)
        {
            _ResultBuilder = iResultBuilder;
        }

        public async Task<object> Execute(object iargument)
        {
            return await _ResultBuilder((Tin)iargument);
        }
    }
}
