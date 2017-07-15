using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    public static class WebViewExtension
    {

        /// <summary>
        ///  returns javascript building strategy
        /// </summary>
        public static IJavascriptObjectBuilderStrategy GetBuildingStrategy(this IWebView @this, IJavascriptSessionCache cache)
        {
            return @this.AllowBulkCreation ? (IJavascriptObjectBuilderStrategy)new JavascriptObjectBulkBuilderStrategy(@this, cache) :
                                             new JavascriptObjectSynchroneousBuilderStrategy(@this, cache);
        }
    }
}
