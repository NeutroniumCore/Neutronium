using MoreCollection.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Neutronium.Core.Infra.Reflection
{
    public class TypePropertyAccessor : IGenericPropertyAcessor
    {
        public IReadOnlyList<PropertyAccessor> ReadProperties { get; }
        public IReadOnlyList<string> AttributeNames { get; }
        public bool HasReadWriteProperties { get; }

        private readonly Dictionary<string, PropertyAccessor> _Properties;

        public TypePropertyAccessor(Type type)
        {
            var readProperties = type.GetPropertyInfoDescriptions().OrderBy(p => p.PropertyInfo.Name)
                                    .Select((prop, index) => new PropertyAccessor(type, prop, index))
                                    .ToArray();

            ReadProperties = readProperties;
            AttributeNames = readProperties.ToArray(p => p.Name);
            _Properties = ReadProperties.ToDictionary(prop => prop.Name, prop => prop);
            HasReadWriteProperties = readProperties.Any(p => p.IsSettable);
        }

        public PropertyAccessor GetAccessor(string propertyName)
        {
            return _Properties.GetOrDefault(propertyName);
        }

        public IndexDescriptor GetIndex(PropertyAccessor propertyAcessor) => new IndexDescriptor(propertyAcessor.Position);
    }
}
