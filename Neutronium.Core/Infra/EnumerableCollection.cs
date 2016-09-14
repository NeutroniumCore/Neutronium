using System;
using System.Collections;
using System.Collections.Generic;

namespace Neutronium.Core.Infra
{
    public static class EnumerableCollection
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> @this, Action<T> ToDo)
        {
            foreach (var el in @this)
            {
                ToDo(el);
            }

            return @this;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> @this, Action<T,int> ToDo)
        {
            int i=0;
            foreach (var el in @this)
            {
                ToDo(el,i++);
            }

            return @this;
        }


        public static Type GetElementType(this IEnumerable collection)
        {
            var typeo = collection.GetType();
            var elementtype = typeo.GetEnumerableBase();
            if (elementtype == null)
                return null;
            return elementtype.GetUnderlyingNullableType() ?? elementtype;
        }

        private static IEnumerable<TResult> ZipInternal<TResult, TSource1, TSource2, TSource3>(IEnumerable<TSource1> enumerable,
                             IEnumerable<TSource2> enumerable2, IEnumerable<TSource3> enumerable3,
                             Func<TSource1, TSource2, TSource3, TResult> Agregate)
        {
            using (var e1 = enumerable.GetEnumerator())
            {
                using (var e2 = enumerable2.GetEnumerator())
                {
                    using (var e3 = enumerable3.GetEnumerator())
                    {
                        while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
                            yield return Agregate(e1.Current, e2.Current, e3.Current);
                    }
                }
            }
        }

        public static IEnumerable<TResult> Zip<TResult, TSource1, TSource2, TSource3>(this IEnumerable<TSource1> enumerable,
                               IEnumerable<TSource2> enumerable2, IEnumerable<TSource3> enumerable3,
                               Func<TSource1, TSource2, TSource3, TResult> Agregate)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (enumerable2 == null)
                throw new ArgumentNullException(nameof(enumerable2));

            if (enumerable3 == null)
                throw new ArgumentNullException(nameof(enumerable3));

            return ZipInternal(enumerable, enumerable2, enumerable3, Agregate);
        }
    }
}
