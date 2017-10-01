using System.Dynamic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal sealed class GlueObjectDynamicObjectBuilder : GlueObjectDynamicBuilder
    {
        public GlueObjectDynamicObjectBuilder(CSharpToJavascriptConverter converter):base(converter)
        {
        }

        public IJsCsGlue Convert(IGlueFactory factory, object @object)
        {
            var propertyAcessor = new DynamicObjectPropertyAccessor((DynamicObject)@object);
            return Convert(factory, @object, propertyAcessor);
        }
    }
}
