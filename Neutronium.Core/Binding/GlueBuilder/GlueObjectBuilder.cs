using System;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal sealed class GlueObjectBuilder
    {
        private readonly IGenericPropertyAcessor _PropertyAccessor;
        private readonly IWebSessionLogger _Logger;
        private readonly CSharpToJavascriptConverter _Converter;

        public GlueObjectBuilder(CSharpToJavascriptConverter converter, IWebSessionLogger logger, Type type) 
        {
            _Converter = converter;
            _Logger = logger;
            _PropertyAccessor = type.GetTypePropertyInfo();
        }

        public IJsCsGlue Convert(IGlueFactory factory, object @object) 
        {
            var result = factory.Build(@object, _PropertyAccessor);
            result.SetAttributes(MapNested(@object));
            return result;
        }

        private IJsCsGlue[] MapNested(object parentObject)
        {
            var properties = _PropertyAccessor.ReadProperties;
            var attributes = new IJsCsGlue[properties.Count];
            var index = 0;

            foreach (var propertyInfo in properties) 
            {
                object childValue = null;
                try 
                {
                    childValue = propertyInfo.Get(parentObject);
                }
                catch (Exception exception)
                {
                    LogIntrospectionError(propertyInfo.Name, parentObject, exception);
                }

                var child = _Converter.Map(childValue).AddRef();
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
