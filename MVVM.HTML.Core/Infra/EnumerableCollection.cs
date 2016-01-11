using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MVVM.HTML.Core.Infra
{
    public static class EnumerableCollection
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> @this, Action<T> ToDo)
        {
            foreach (T el in @this)
            {
                ToDo(el);
            }

            return @this;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> @this, Action<T,int> ToDo)
        {
            int i=0;
            foreach (T el in @this)
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
    }
}
