using System;

namespace Neutronium.Core.Infra
{
    public static class LazyExtensions
    {
        public static bool LazyDo<T>(this Lazy<T> lazy, Action<T> action)
        {
            if (!lazy.IsValueCreated)
                return false;

            action(lazy.Value);
            return true;
        }
    }
}
