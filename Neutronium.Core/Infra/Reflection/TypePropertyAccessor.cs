using MoreCollection.Extensions;
using System;
using System.Collections.Generic;
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

        private TypePropertyAccessor(Type type)
        {
            var readProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                    .Where(p => p.CanRead && p.GetGetMethod(false) != null)
                                    .OrderBy(p => p.Name)
                                    .Select((prop, index) => new PropertyAccessor(type, prop, index))
                                    .ToArray();
            ReadProperties = readProperties;
            AttributeNames = readProperties.ToArray(p => p.Name);
            _Properties = ReadProperties.ToDictionary(prop => prop.Name, prop => prop);
            HasReadWriteProperties = readProperties.Any(p => p.IsSettable);
        }

        public static IGenericPropertyAcessor FromType(Type type)
        {
            return new TypePropertyAccessor(type);
        }

        public PropertyAccessor GetAccessor(string propertyName)
        {
            return _Properties.GetOrDefault(propertyName);
        }

        public IndexDescriptor GetIndex(PropertyAccessor propertyAcessor) => new IndexDescriptor(propertyAcessor.Position);
    }
}
