using System;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Factory;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal sealed class GlueObjectBuilder
    {
        private readonly TypePropertyAccessor _TypePropertyAccessor;
        private readonly IWebSessionLogger _Logger;
        private readonly CSharpToJavascriptConverter _Converter;

        public GlueObjectBuilder(CSharpToJavascriptConverter converter, IWebSessionLogger logger, Type objectType) 
        {
            _Converter = converter;
            _Logger = logger;
            _TypePropertyAccessor = objectType.GetTypePropertyInfo();
        }

        public IJsCsGlue Convert(IGlueFactory factory, object @object) 
        {
            var result = factory.Build(@object, _TypePropertyAccessor);
            result.SetAttributes(MapNested(@object));
            return result;
        }

        private IJsCsGlue[] MapNested(object parentObject)
        {
            var properties = _TypePropertyAccessor.ReadProperties;
            var attributes = new IJsCsGlue[properties.Length];
            var index = 0;

            foreach (var propertyInfo in properties) 
            {
                var propertyName = propertyInfo.Name;
                object childvalue = null;
                try 
                {
                    childvalue = propertyInfo.Get(parentObject);
                }
                catch (Exception exception)
                {
                    LogIntrospectionError(propertyName, parentObject, exception);
                }

                var child = _Converter.Map(childvalue).AddRef();
                attributes[index++] = child;
            }
            return attributes;
        }

        private void LogIntrospectionError(string propertyName, object parentObject, Exception exception) 
        {
            _Logger.Info(() => $"Unable to convert property {propertyName} from {parentObject} of type {parentObject.GetType().FullName} exception {exception.InnerException}");
        }
    }
}
