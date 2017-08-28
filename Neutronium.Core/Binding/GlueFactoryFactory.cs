using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Mapped;

namespace Neutronium.Core.Binding
{
    internal static class GlueFactoryFactory
    {
        internal static IGlueFactory GetFactory(HtmlViewContext context, ICSharpToJsCache cacher, IJavascriptToCSharpConverter converter)
        {
            var isMapping = context.JavascriptFrameworkIsMappingObject;
            return isMapping ? (IGlueFactory)new GlueMappingFactory(context, cacher, converter) : new GlueFactory(context, cacher, converter);
        }
    }
}
