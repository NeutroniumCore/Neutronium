using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Binding.Mapper;

namespace Neutronium.Core.Binding
{
    internal static class GlueFactoryFactory
    {
        internal static IGlueFactory GetFactory(HtmlViewContext context, ICSharpToJsCache cacher, ICSharpUnrootedObjectManager manager, 
            IJavascriptToGlueMapper converter, ObjectChangesListener onListener)
        {
            var isMapping = context.JavascriptFrameworkIsMappingObject;
            return isMapping ? (IGlueFactory)new GlueMappingFactory(context, cacher, manager, converter, onListener) : new GlueFactory(context, cacher, manager, converter, onListener);
        }
    }
}
