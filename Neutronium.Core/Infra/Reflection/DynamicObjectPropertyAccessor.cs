using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using MoreCollection.Extensions;

namespace Neutronium.Core.Infra.Reflection
{
    public class DynamicObjectPropertyAccessor : IGenericPropertyAcessor
    {
        public IReadOnlyList<PropertyAccessor> ReadProperties => _ReadProperties;
        public IReadOnlyList<string> AttributeNames => _AttributeNames;
        public bool HasReadWriteProperties => true;

        private readonly List<PropertyAccessor> _ReadProperties;
        private readonly List<string> _AttributeNames;
        private readonly Dictionary<string, PropertyAccessor> _PropertyAccessoresDictionary;

        internal DynamicObjectPropertyAccessor(DynamicObject @dynamicObject)
        {
            var type = @dynamicObject.GetType();

            var staticPropertyInfo = type.GetPropertyInfoDescriptions().ToList();

            var staticAttributesNames = new HashSet<string>(staticPropertyInfo.Select(p => p.PropertyInfo.Name));
            var dynamicAttributesNames = @dynamicObject.GetDynamicMemberNames().Where(d => !staticAttributesNames.Contains(d)).ToList();

            var staticPropertyAccessores = staticPropertyInfo.Select(property => new PropertyBuilder(property.PropertyInfo.Name, (pn, index) => new PropertyAccessor(type, property, index)));
            var dynamicPropertyAccessores = dynamicAttributesNames.Select(name => new PropertyBuilder(name, (pn, index) => PropertyAccessor.FromDynamicObject(type, pn, index)));

            _ReadProperties = staticPropertyAccessores.Concat(dynamicPropertyAccessores)
                                    .OrderBy(p => p.Name)
                                    .Select((p, i) => p.Builder(p.Name, i))
                                    .ToList();

            _PropertyAccessoresDictionary = _ReadProperties.ToDictionary(p => p.Name);
            _AttributeNames = _ReadProperties.Select(p => p.Name).ToList();
        }

        private struct PropertyBuilder
        {
            public string Name { get; }
            public Func<string, int,PropertyAccessor> Builder { get; }

            public PropertyBuilder(string name, Func<string, int, PropertyAccessor> builder)
            {
                Name = name;
                Builder = builder;
            }
        }

        public PropertyAccessor GetAccessor(string propertyName)
        {
            return _PropertyAccessoresDictionary.GetOrDefault(propertyName); ;
        }

        public IndexDescriptor GetIndex(PropertyAccessor propertyAcessor) => new IndexDescriptor(propertyAcessor.Position);
    }
}
