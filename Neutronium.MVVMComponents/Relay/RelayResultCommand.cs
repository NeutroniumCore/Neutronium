using System;

namespace Neutronium.MVVMComponents.Relay
{
    public static class RelayResultCommand
    {
        public static IResultCommand Create<TIn, TResult>(Func<TIn, TResult> iFunction)
        {
            return new RelayResultCommand<TIn, TResult>(iFunction);
        }

        public static IResultCommand Create<TResult>(Func<TResult> iFunction)
        {
            return new RelayResultCommand<TResult>(iFunction);
        }
    }
}
