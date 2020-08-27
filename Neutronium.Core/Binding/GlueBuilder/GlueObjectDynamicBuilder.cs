using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal abstract class GlueObjectDynamicBuilder
    {
        private readonly ICSharpToGlueMapper _Converter;

        protected GlueObjectDynamicBuilder(ICSharpToGlueMapper converter)
        {
            _Converter = converter;
        }

        protected IJsCsGlue Convert(IGlueFactory factory, object @object, IGenericPropertyAcessor propertyAccessor)
        {
            var result = factory.Build(@object, propertyAccessor);
            result.SetAttributes(MapNested(@object, propertyAccessor));
            return result;
        }

        private List<IJsCsGlue> MapNested(object parentObject, IGenericPropertyAcessor propertyAccessor)
        {
            var properties = propertyAccessor.ReadProperties;
            var attributes = new List<IJsCsGlue>(properties.Count);
            foreach (var propertyInfo in properties) 
            {
                var childValue = propertyInfo.Get(parentObject);
                var child = _Converter.Map(childValue).AddRef();
                attributes.Add(child);
            }
            return attributes;
        }
    }
}
