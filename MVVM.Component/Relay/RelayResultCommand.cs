using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.Component.Infra;

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

        public Task<object> Execute(object iargument)
        {
            return _ResultBuilder((Tin)iargument).Convert();
        }
    }

    public class RelayResultCommand<TResult> : IResultCommand
    {

        private Func< TResult> _Function;
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

    public static class RelayResultCommand
    {
        public static IResultCommand Create<Tin, TResult>(Func<Tin, TResult> iFunction)
        {
            return new RelayResultCommand<Tin, TResult>(iFunction);
        }

        public static IResultCommand Create<TResult>(Func<TResult> iFunction)
        {
            return new RelayResultCommand<TResult>(iFunction);
        }
    }
}
