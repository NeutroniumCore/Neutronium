using System;

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
