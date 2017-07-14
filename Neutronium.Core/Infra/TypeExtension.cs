using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Infra
{
    public static class TypeExtender
    {
        private static readonly Type NullableType = typeof(Nullable<>);
        private static readonly Type EnumerableType = typeof(IEnumerable<>);
        private static readonly Type UInt16Type = typeof(UInt16);
        private static readonly Type UInt32Type = typeof(UInt32);
        private static readonly Type UInt64Type = typeof(UInt64);
        
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

            if (type.GetGenericTypeDefinition() == EnumerableType)
                return type.GetGenericArguments()[0];

            var types = type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == EnumerableType).ToArray();
            // Only support collections that implement IEnumerable<T> once.
            return types.Length == 1 ? types[0].GetGenericArguments()[0] : null;
        }

        public static Type GetUnderlyingNullableType(this Type type)
        {
            if (type == null)
                return null;

            if (!type.IsGenericType)
                return null;

            return type.GetGenericTypeDefinition() == NullableType ? type.GetGenericArguments()[0] : null;
        }

        public static Type GetUnderlyingType(this Type type) => GetUnderlyingNullableType(type) ?? type;

        public static bool IsUnsigned(this Type targetType) 
        {
            return (targetType != null) && ((targetType == UInt16Type) || (targetType == UInt32Type) || (targetType == UInt64Type));
        }
    }
}
