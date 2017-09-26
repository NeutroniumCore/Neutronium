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
        private static readonly MethodInfo _BuildAccessorDictionary = typeof(TypePropertyAccessor).GetMethod(nameof(FromStringDictionary), BindingFlags.Static | BindingFlags.NonPublic);

        private TypePropertyAccessor(Type type): 
            this(type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanRead && p.GetGetMethod(false) != null)
                        .OrderBy(p => p.Name)
                        .Select((prop,index) => new PropertyAccessor(type, prop, index)))
        {
        }

        private TypePropertyAccessor(IEnumerable<PropertyAccessor> propertyAcessores)
        {
            ReadProperties = propertyAcessores.ToArray();
            AttributeNames = ReadProperties.ToArray(p => p.Name);
            _ReadProperties = ReadProperties.ToDictionary(prop => prop.Name, prop => prop);
            HasReadWriteProperties = _ReadProperties.Any(p => p.Value.IsSettable);
        }

        public static TypePropertyAccessor FromType(Type type)
        {
            return new TypePropertyAccessor(type);
        }

        public static TypePropertyAccessor FromStringDictionary(object @object, Type type)
        {
            var builder = _BuildAccessorDictionary.MakeGenericMethod(type);
            return (TypePropertyAccessor)builder.Invoke(null, new[] { @object });
        }

        private static TypePropertyAccessor FromStringDictionary<T>(object @object)
        {
            var accessores = ((IDictionary<string, T>) @object).Keys.OrderBy(p => p).Select(PropertyAccessor.FromDictionary<T>);
            return new TypePropertyAccessor(accessores);
        }

        public PropertyAccessor GetAccessor(string propertyName)
        {
            return _ReadProperties.GetOrDefault(propertyName);
        }
    }
}
