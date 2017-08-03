using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    public static class WebViewExtension
    {

        /// <summary>
        ///  returns javascript building strategy
        /// </summary>
        public static IJavascriptObjectBuilderStrategy GetBuildingStrategy(this IWebView @this, IJavascriptSessionCache cache, bool mapping)
        {
            return @this.AllowBulkCreation ? (IJavascriptObjectBuilderStrategy)new JavascriptObjectBulkBuilderStrategy(@this, cache, mapping) :
                                             new JavascriptObjectSynchroneousBuilderStrategy(@this, cache, mapping);
        }
    }
}
