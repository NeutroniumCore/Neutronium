using System.Collections.Generic;

namespace Neutronium.Core.Infra
{
    public static class DictionaryExtensions
    {
        public static object GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue result;
            if (dictionary.TryGetValue(key, out result))
                return result;

            return null;
        }
    }
}
