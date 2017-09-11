using MoreCollection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neutronium.Core.Infra.Reflection
{
    public class TypePropertyAccessor
    {
        public PropertyAccessor[] ReadProperties { get; }
        public string[] AttributeNames { get; }
        public bool HasReadWriteProperties { get; }
        private readonly IDictionary<string, PropertyAccessor> _ReadProperties;

        public TypePropertyAccessor(Type type)
        {
            var rank = 0;
            ReadProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetGetMethod(false) != null)
                .OrderBy(p => p.Name).Select(prop => new PropertyAccessor(type, prop, rank++))
                .ToArray();

            AttributeNames = ReadProperties.Select(p => p.Name).ToArray();

            _ReadProperties = ReadProperties.ToDictionary(prop => prop.Name, prop => prop);

            HasReadWriteProperties = _ReadProperties.Any(p => p.Value.IsSettable);
        }

        public PropertyAccessor GetAccessor(string propertyName)
        {
            return _ReadProperties.GetOrDefault(propertyName);
        }
    }
}
