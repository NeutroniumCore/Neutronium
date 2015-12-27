using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Component
{
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
