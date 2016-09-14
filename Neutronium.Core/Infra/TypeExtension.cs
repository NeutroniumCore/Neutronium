using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Infra
{
    public static class TypeExtender
    {
        public static IEnumerable<Type> GetBaseTypes(this Type type) 
        {
            if (type == null) throw new ArgumentNullException();
            yield return type;

            while ((type = type.BaseType) != null)
            {
                yield return type;
            }
        }

        public static Type GetEnumerableBase(this Type type)
        {
            if (type == null)
                return null;

            if (!type.IsGenericType)
                return null;

            if (type.GetGenericTypeDefinition() == typeof (IEnumerable<>))
                return type.GetGenericArguments()[0];

            var types = type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IEnumerable<>)).ToArray();
            // Only support collections that implement IEnumerable<T> once.
            return types.Length == 1 ? types[0].GetGenericArguments()[0] : null;
        }

        public static Type GetUnderlyingNullableType(this Type type)
        {
            if (type == null)
                return null;

            if (!type.IsGenericType)
                return null;

            return type.GetGenericTypeDefinition() == typeof (Nullable<>) ? type.GetGenericArguments()[0] : null;
        }

        public static bool IsUnsigned(this Type targetType) 
        {
            return (targetType != null) && ((targetType == typeof(UInt16)) || (targetType == typeof(UInt32)) || (targetType == typeof(UInt64)));
        }
    }
}
