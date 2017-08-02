using System;
using System.Collections;
using System.Collections.Generic;

namespace Neutronium.Core.Infra
{
    public static class EnumerableCollection
    {
        public static Type GetElementType(this IEnumerable collection)
        {
            var type = collection.GetType();
            var elementtype = type.GetEnumerableBase();
            if (elementtype == null)
                return null;
            return elementtype.GetUnderlyingType();
        }

        public static void ZipForEach<TSource1, TSource2>(this IEnumerable<TSource1> enumerable,
               IEnumerable<TSource2> enumerable2, Action<TSource1, TSource2> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (enumerable2 == null)
                throw new ArgumentNullException(nameof(enumerable2));

            using (var e1 = enumerable.GetEnumerator())
            {
                using (var e2 = enumerable2.GetEnumerator())
                {
                    while (e1.MoveNext() && e2.MoveNext())
                        action(e1.Current, e2.Current);
                }
            }
        }
    }
}
