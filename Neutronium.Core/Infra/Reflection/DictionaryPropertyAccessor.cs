using MoreCollection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Infra.Reflection 
{
    public class DictionaryPropertyAccessor<T> : IGenericPropertyAcessor 
    {
        public IReadOnlyList<PropertyAccessor> ReadProperties => _ReadProperties;
        public IReadOnlyList<string> AttributeNames => _AttributeNames;
        public bool HasReadWriteProperties => true;

        private readonly IDictionary<string, T> _Dictionary;
        private readonly IDictionary<string, PropertyAccessor> _PropertyAccessoresDictionary;
        public readonly List<PropertyAccessor> _ReadProperties;
        public readonly List<string> _AttributeNames;

        internal DictionaryPropertyAccessor(IDictionary<string,T> dictionary) 
        {
            _Dictionary = dictionary;
            var readProperties = _Dictionary.Keys.OrderBy(p => p).Select(PropertyAccessor.FromDictionary<T>).ToList();
            _ReadProperties = readProperties;
            _AttributeNames = readProperties.Select(p => p.Name).ToList();
            _PropertyAccessoresDictionary = ReadProperties.ToDictionary(prop => prop.Name, prop => prop);
        }

        public PropertyAccessor GetAccessor(string propertyName)
        {
            return _PropertyAccessoresDictionary.GetOrAddEntity(propertyName, pn => PropertyAccessor.FromDictionary<T>(pn, -1)); ;
        }

        public bool Update<TEntity>(IList<TEntity> attributes, PropertyAccessor propertyAcessor, TEntity jsCsGlue) 
        {
            var index = propertyAcessor.Position;
            if (index > 0) 
            {
                attributes[index] = jsCsGlue;
                return false;
            }

            var name = propertyAcessor.Name;
            index = ~_AttributeNames.BinarySearch(name);
            _AttributeNames.Insert(index, name);
            _ReadProperties.Insert(index, propertyAcessor);
            for(var i = index; i< _ReadProperties.Count; i++) 
            {
                propertyAcessor.Position = i;
            }

            attributes.Insert(index, jsCsGlue);
            return true;
        }

        public IndexDescriptor GetIndex(PropertyAccessor propertyAcessor) 
        {
            var index = propertyAcessor.Position;
            if (index > 0)
                return new IndexDescriptor(index);

            var name = propertyAcessor.Name;
            index = ~_AttributeNames.BinarySearch(name);
            _AttributeNames.Insert(index, name);
            _ReadProperties.Insert(index, propertyAcessor);
            for (var i = index; i < _ReadProperties.Count; i++) 
            {
                propertyAcessor.Position = i;
            }
            return new IndexDescriptor(index, true);
        }
    }

    public class DictionaryPropertyAccessor
    {
        private static readonly MethodInfo _BuildAccessorDictionary = typeof(DictionaryPropertyAccessor).GetMethod(nameof(FromStringDictionary), BindingFlags.Static | BindingFlags.NonPublic);

        public static IGenericPropertyAcessor FromStringDictionary(object @object, Type type) 
        {
            var builder = _BuildAccessorDictionary.MakeGenericMethod(type);
            return (IGenericPropertyAcessor)builder.Invoke(null, new[] { @object });
        }

        private static IGenericPropertyAcessor FromStringDictionary<T>(object @object) 
        {
            var dictionary = ((IDictionary<string, T>)@object);
            return new DictionaryPropertyAccessor<T>(dictionary);
        }
    }
}
