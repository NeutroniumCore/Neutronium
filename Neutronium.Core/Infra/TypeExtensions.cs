using Neutronium.Core.Infra.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Neutronium.Core.Infra
{
    public static class TypeExtensions
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
            if ((type == null) || (type == Types.String))
                return null;

            if (type.IsArray)
                return type.GetElementType();

            if ((type.IsGenericType) && (type.GetGenericTypeDefinition() == Types.Enumerable))
                return type.GetGenericArguments()[0];

            return GetInterfaceGenericType(type, Types.Enumerable);
        }

        public static Type GetUnderlyingNullableType(this Type type)
        {
            if (type == null)
                return null;

            if (!type.IsGenericType)
                return null;

            return type.GetGenericTypeDefinition() == Types.Nullable ? type.GetGenericArguments()[0] : null;
        }

        public static Type GetUnderlyingType(this Type type) => GetUnderlyingNullableType(type) ?? type;

        public static bool IsUnsigned(this Type targetType) 
        {
            return (targetType != null) && ((targetType == Types.ULong) || (targetType == Types.UShort) || (targetType == Types.Uint));
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
                if ((!interfaceType.IsGenericType) || (interfaceType.GetGenericTypeDefinition() != Types.Dictionary))
                    continue;

                var arguments = interfaceType.GetGenericArguments();
                if (arguments[0] == Types.String)
                    return arguments[1];
            }

            return null;
        }

        public static Type GetInterfaceGenericType(this Type type, Type genericType) 
        {
            if (type == null)
                return null;

            foreach (var interfaceType in type.GetInterfaces()) 
            {
                if ((!interfaceType.IsGenericType) || (interfaceType.GetGenericTypeDefinition() != genericType))
                    continue;

                var arguments = interfaceType.GetGenericArguments();
                return arguments[0];
            }

            return null;
        }
    }
}
