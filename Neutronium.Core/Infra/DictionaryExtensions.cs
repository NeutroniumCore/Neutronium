using System.Collections.Generic;

namespace Neutronium.Core.Infra
{
    public static class DictionaryExtensions
    {
        public static object GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var result))
                return result;

            return null;
        }
    }
}
