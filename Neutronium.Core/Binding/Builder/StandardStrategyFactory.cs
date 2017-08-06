using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    public class StandardStrategyFactory : IJavascriptObjectBuilderStrategyFactory
    {

        /// <summary>
        ///  returns javascript building strategy
        /// </summary>
        public IJavascriptObjectBuilderStrategy GetStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping)
        {
            return webView.AllowBulkCreation ? (IJavascriptObjectBuilderStrategy)new JavascriptObjectMixtBuilderStrategy(webView, cache, mapping) :
                                             new JavascriptObjectSynchroneousBuilderStrategy(webView, cache, mapping);
        }
    }

}
