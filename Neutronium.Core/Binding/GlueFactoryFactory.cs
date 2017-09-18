using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding
{
    internal static class GlueFactoryFactory
    {
        internal static IGlueFactory GetFactory(HtmlViewContext context, ICSharpToJsCache cacher, IJavascriptToCSharpConverter converter, ObjectChangesListener onListener)
        {
            var isMapping = context.JavascriptFrameworkIsMappingObject;
            return isMapping ? (IGlueFactory)new GlueMappingFactory(context, cacher, converter, onListener) : new GlueFactory(context, cacher, converter, onListener);
        }
    }
}
