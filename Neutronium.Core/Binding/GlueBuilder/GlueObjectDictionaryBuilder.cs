using System;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal sealed class GlueObjectDictionaryBuilder
    {
        private readonly Type _TargetType;
        private readonly CSharpToJavascriptConverter _Converter;

        public GlueObjectDictionaryBuilder(CSharpToJavascriptConverter converter, Type targetType) 
        {
            _Converter = converter;
            _TargetType = targetType;
        }

        public IJsCsGlue Convert(IGlueFactory factory, object @object)
        {
            var propertyAcessor = DictionaryPropertyAccessor.FromStringDictionary(@object, _TargetType);
            var result = factory.Build(@object, propertyAcessor);
            result.SetAttributes(MapNested(@object, propertyAcessor));
            return result;
        }

        private List<IJsCsGlue> MapNested(object parentObject, IGenericPropertyAcessor propertyAcessor)
        {
            var properties = propertyAcessor.ReadProperties;
            var attributes = new List<IJsCsGlue>(properties.Count);
            foreach (var propertyInfo in properties) 
            {
                var childvalue = propertyInfo.Get(parentObject);
                var child = _Converter.Map(childvalue).AddRef();
                attributes.Add(child);
            }
            return attributes;
        }
    }
}
