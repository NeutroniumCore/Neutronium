using System;
using System.Collections.Generic;
using System.Linq;

namespace MVVM.HTML.Core.Infra
{
    public static class TypeExtender
    {
        static public IEnumerable<Type> GetBaseTypes(this Type itype)
        {
            if (itype == null) throw new ArgumentNullException();
            yield return itype;

            while ((itype = itype.BaseType) != null)
            {
                yield return itype;
            }
        }

        static public Type GetEnumerableBase(this Type itype)
        {
            if (itype == null)
                return null;

            if (!itype.IsGenericType)
                return null;

            if (itype.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return itype.GetGenericArguments()[0];

            var types = itype.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).ToArray();
            // Only support collections that implement IEnumerable<T> once.
            return types.Length == 1 ? types[0].GetGenericArguments()[0] : null;
        }

        static public Type GetUnderlyingNullableType(this Type itype)
        {
            if (itype == null)
                return null;

            if (!itype.IsGenericType)
                return null;

            if (itype.GetGenericTypeDefinition() == typeof(Nullable<>))
                return itype.GetGenericArguments()[0];

            return null;
        }
    }

}
