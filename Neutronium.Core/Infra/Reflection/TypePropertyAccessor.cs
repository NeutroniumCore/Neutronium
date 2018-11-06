using MoreCollection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Infra.Reflection
{
    public sealed class TypePropertyAccessor : IGenericPropertyAcessor
    {
        public IReadOnlyList<PropertyAccessor> ReadProperties { get; }
        public IReadOnlyList<string> AttributeNames { get; }
        public ObjectObservability Observability { get; }

        private readonly Dictionary<string, PropertyAccessor> _Properties;

        public TypePropertyAccessor(Type type)
        {
            var readProperties = type.GetPropertyInfoDescriptions().OrderBy(p => p.PropertyInfo.Name)
                                    .Select((prop, index) => new PropertyAccessor(type, prop, index))
                                    .ToArray();

            ReadProperties = readProperties;
            AttributeNames = readProperties.ToArray(p => p.Name);
            _Properties = ReadProperties.ToDictionary(prop => prop.Name, prop => prop);
            Observability = GetObjectObservability(type);
        }

        private ObjectObservability GetObjectObservability(Type type)
        {
            var observability = ObjectObservability.None;
            if (ReadProperties.All(p => !p.IsSettable))
                observability = observability | ObjectObservability.ReadOnly;
            if (Types.NotifyPropertyChanged.IsAssignableFrom(type))
                observability = observability | ObjectObservability.ImplementNotifyPropertyChanged;
            return observability;
        }

        public PropertyAccessor GetAccessor(string propertyName)
        {
            return _Properties.GetOrDefault(propertyName);
        }

        public IndexDescriptor GetIndex(PropertyAccessor propertyAcessor) => new IndexDescriptor(propertyAcessor.Position);
    }
}
