using System;
using System.Collections;

namespace Neutronium.Core.Infra
{
    public static class EnumerableCollection
    {
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
