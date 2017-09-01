using System;
using System.Collections;
using System.Collections.Concurrent;

namespace Neutronium.Core.Infra
{
    public static class EnumerableExtension
    {
        private static readonly ConcurrentDictionary<Type, Type> _EnumerableElementType = new ConcurrentDictionary<Type, Type>();

        public static Type GetElementType(this IEnumerable collection)
        {
            var type = collection.GetType();

            return _EnumerableElementType.GetOrAdd(type, t =>
            {
                var elementType = t.GetEnumerableBase();
                return elementType?.GetUnderlyingType();
            });
        }
    }
}
