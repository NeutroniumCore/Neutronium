using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.SessionManagement;

namespace Neutronium.Core.Binding.JavascriptFrameworkMapper
{
    internal static class JavascriptFrameworkMapper
    {
        internal static IJavascriptFrameworkMapper GetMapper(this HtmlViewContext context, IInternalSessionCache sessionCache, IJavascriptObjectBuilderStrategy builderStrategy)
        {
            var mapping = context.JavascriptFrameworkIsMappingObject;
            return mapping
                ? (IJavascriptFrameworkMapper)new JavascriptFrameworkRealMapper(context, context.JavascriptSessionInjector, sessionCache, builderStrategy)
                : new JavascriptFrameworkNoMapper(context, sessionCache, builderStrategy);
        }
    }
}
