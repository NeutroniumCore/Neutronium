using Neutronium.Core.Infra.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Infra
{
    public static class TypeExtensions
    {
        private static readonly Type _NullableType = typeof(Nullable<>);
        private static readonly Type _EnumerableType = typeof(IEnumerable<>);
        private static readonly Type _DictionaryType = typeof(IDictionary<,>);
        private static readonly Type _StringType = typeof(string);
        private static readonly Type _UInt16Type = typeof(UInt16);
        private static readonly Type _UInt32Type = typeof(UInt32);
        private static readonly Type _UInt64Type = typeof(UInt64);

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

            if (type.IsArray)
                return type.GetElementType();

            if (!type.IsGenericType)
                return null;

            if (type.GetGenericTypeDefinition() == _EnumerableType)
                return type.GetGenericArguments()[0];

            foreach (var interfaceType in type.GetInterfaces())
            {
                if ((interfaceType.IsGenericType)  && (interfaceType.GetGenericTypeDefinition() == _EnumerableType))
                    return interfaceType.GetGenericArguments()[0];
            }

            return null;
        }

        public static Type GetUnderlyingNullableType(this Type type)
        {
            if (type == null)
                return null;

            if (!type.IsGenericType)
                return null;

            return type.GetGenericTypeDefinition() == _NullableType ? type.GetGenericArguments()[0] : null;
        }

        public static Type GetUnderlyingType(this Type type) => GetUnderlyingNullableType(type) ?? type;

        public static bool IsUnsigned(this Type targetType) 
        {
            return (targetType != null) && ((targetType == _UInt16Type) || (targetType == _UInt32Type) || (targetType == _UInt64Type));
        }

        private static readonly ConcurrentDictionary<Type, IGenericPropertyAcessor> _TypePropertyInfos = new ConcurrentDictionary<Type, IGenericPropertyAcessor>();
        internal static IGenericPropertyAcessor GetTypePropertyInfo(this Type @type)
        {
            return _TypePropertyInfos.GetOrAdd(@type, t => new TypePropertyAccessor(t));
        }

        public static Type GetDictionaryStringValueType(this Type type)
        {
            if (type == null)
                return null;

            foreach (var interfaceType in type.GetInterfaces())
            {
                if ((!interfaceType.IsGenericType) || (interfaceType.GetGenericTypeDefinition() != _DictionaryType))
                    continue;

                var arguments = interfaceType.GetGenericArguments();
                if (arguments[0] == _StringType)
                    return arguments[1];
            }

            return null;
        }
    }
}
