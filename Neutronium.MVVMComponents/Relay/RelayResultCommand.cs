using System;

namespace Neutronium.MVVMComponents.Relay
{
    public static class RelayResultCommand
    {
        public static IResultCommand Create<TIn, TResult>(Func<TIn, TResult> function)
        {
            return new RelayResultCommand<TIn, TResult>(function);
        }

        public static IResultCommand Create<TResult>(Func<TResult> function)
        {
            return new RelayResultCommand<TResult>(function);
        }
    }
}
