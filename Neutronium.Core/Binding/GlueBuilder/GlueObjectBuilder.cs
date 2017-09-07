using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Factory;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal sealed class GlueObjectBuilder : ICsToGlueConverter
    {
        private readonly IGlueFactory _GlueFactory;
        private readonly KeyValuePair<string, PropertyAccessor>[] _Properties;
        private readonly IWebSessionLogger _Logger;
        private readonly CSharpToJavascriptConverter _Converter;

        public GlueObjectBuilder(IGlueFactory factory, CSharpToJavascriptConverter converter, IWebSessionLogger logger, Type objectType) 
        {
            _GlueFactory = factory;
            _Converter = converter;
            _Logger = logger;
            _Properties = objectType.GetReadProperties().ToArray();
        }

        public IJsCsGlue Convert(object @object) 
        {
            var result = _GlueFactory.Build(@object);
            result.SetAttributes(MapNested(@object));
            return result;
        }

        private AttributeDescription[] MapNested(object parentObject) 
        {
            var attributes = new AttributeDescription[_Properties.Length];
            var index = 0;

            foreach (var propertyInfo in _Properties) 
            {
                var propertyName = propertyInfo.Key;
                object childvalue = null;
                try 
                {
                    childvalue = propertyInfo.Value.Get(parentObject);
                }
                catch (Exception e) 
                {
                    _Logger.Info(() => $"Unable to convert property {propertyName} from {parentObject} of type {parentObject.GetType().FullName} exception {e.InnerException}");
                }

                var child = _Converter.Map(childvalue).AddRef();
                attributes[index++] = new AttributeDescription(propertyName, child);
            }
            return attributes;
        }
    }
}
