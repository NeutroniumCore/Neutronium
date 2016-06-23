using System;
using System.Collections.Generic;

namespace MVVM.HTML.Core.Infra
{
    public static class DictionaryExtension
    {
        public static TValue FindOrCreateEntity<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac)
        {
            TValue res;
            if (dic.TryGetValue(key, out res))
                return res;

            res = Fac(key);
            dic.Add(key, res);
            return res;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            TValue res;
            dic.TryGetValue(key, out res);
            return res;
        }
    }
}
