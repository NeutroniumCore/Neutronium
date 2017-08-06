using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Mapped;

namespace Neutronium.Core.Binding
{
    internal static class GlueFactoryFactory
    {
        internal static IGlueFactory GetFactory(HTMLViewContext context, IJavascriptToCSharpConverter converter)
        {
            var isMapping = context.JavascriptFrameworkIsMappingObject;
            return isMapping ? (IGlueFactory)new GlueMappingFactory(context, converter) : new GlueFactory(context, converter);
        }
    }
}
