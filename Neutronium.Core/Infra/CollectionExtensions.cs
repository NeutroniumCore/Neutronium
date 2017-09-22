using System;
using System.Collections.Generic;

namespace Neutronium.Core.Infra
{
    public static class CollectionExtensions
    {
        public static TResult[] ToArray<TSource, TResult>(this ICollection<TSource> collection, Func<TSource, TResult> select)
        {
            var res = new TResult[collection.Count];
            var i = 0;
            foreach (var item in collection)
            {
                res[i++] = select(item);
            }
            return res;
        }
    }
}
