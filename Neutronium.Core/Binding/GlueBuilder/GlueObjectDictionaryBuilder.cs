using System;
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
            var propertyAcessor = TypePropertyAccessor.FromStringDictionary(@object, _TargetType);
            var result = factory.Build(@object, propertyAcessor);
            result.SetAttributes(MapNested(@object, propertyAcessor));
            return result;
        }

        private IJsCsGlue[] MapNested(object parentObject, TypePropertyAccessor propertyAcessor)
        {
            var properties = propertyAcessor.ReadProperties;
            var attributes = new IJsCsGlue[properties.Length];
            var index = 0;
            foreach (var propertyInfo in properties) 
            {
                var childvalue = propertyInfo.Get(parentObject);
                var child = _Converter.Map(childvalue).AddRef();
                attributes[index++] = child;
            }
            return attributes;
        }
    }
}
