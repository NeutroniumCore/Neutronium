using MoreCollection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neutronium.Core.Infra.Reflection
{
    internal class TypePropertyAccessor
    {
        public ICollection<KeyValuePair<string, PropertyAccessor>> ReadProperties => _ReadProperties;
        public bool HasReadWriteProperties { get; }
        private readonly IDictionary<string, PropertyAccessor> _ReadProperties;

        public TypePropertyAccessor(Type type)
        {
            _ReadProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetGetMethod(false) != null)
                .ToDictionary(prop => prop.Name, prop => new PropertyAccessor(type, prop));

            HasReadWriteProperties = ReadProperties.Any(p => p.Value.IsSettable);
        }

        public PropertyAccessor GetAccessor(string propertyName)
        {
            return _ReadProperties.GetOrDefault(propertyName);
        }
    }
}
