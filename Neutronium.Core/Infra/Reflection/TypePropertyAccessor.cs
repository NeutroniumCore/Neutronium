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
            _ReadProperties = new SortedDictionary<string, PropertyAccessor>();

            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetGetMethod(false) != null)
                .ForEach(prop => _ReadProperties.Add(prop.Name, new PropertyAccessor(type, prop)));

            HasReadWriteProperties = ReadProperties.Any(p => p.Value.IsSettable);
        }

        public PropertyAccessor GetAccessor(string propertyName)
        {
            return _ReadProperties.GetOrDefault(propertyName);
        }
    }
}
