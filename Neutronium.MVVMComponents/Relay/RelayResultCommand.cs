using System;

namespace Neutronium.MVVMComponents.Relay
{
    /// <summary>
    /// IResultCommand factory
    /// </summary>
    public static class RelayResultCommand
    {
        /// <summary>
        /// Create a IResultCommand from given function
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <param name="function">
        /// </param>
        /// <returns></returns>
        public static IResultCommand Create<TIn, TResult>(Func<TIn, TResult> function)
        {
            return new RelayResultCommand<TIn, TResult>(function);
        }

        /// <summary>
        /// Create a IResultCommand from given function
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        public static IResultCommand Create<TResult>(Func<TResult> function)
        {
            return new RelayResultCommand<TResult>(function);
        }
    }
}
