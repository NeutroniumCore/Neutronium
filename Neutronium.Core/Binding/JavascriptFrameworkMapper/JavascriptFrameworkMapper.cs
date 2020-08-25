using Neutronium.Core.Binding.Builder;

namespace Neutronium.Core.Binding.JavascriptFrameworkMapper
{
    internal static class JavascriptFrameworkMapper
    {
        internal static IJavascriptFrameworkMapper GetMapper(this HtmlViewContext context, SessionCacher sessionCache, IJavascriptObjectBuilderStrategy builderStrategy)
        {
            var mapping = context.JavascriptFrameworkIsMappingObject;
            return mapping
                ? (IJavascriptFrameworkMapper)new JavascriptFrameworkRealMapper(context, context.JavascriptSessionInjector, sessionCache, builderStrategy)
                : new JavascriptFrameworkNoMapper(context, sessionCache, builderStrategy);
        }
    }
}
