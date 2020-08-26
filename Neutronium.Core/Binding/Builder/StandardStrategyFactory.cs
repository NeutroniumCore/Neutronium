using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    public sealed class StandardStrategyFactory : IJavascriptObjectBuilderStrategyFactory
    {

        /// <summary>
        ///  returns javascript building strategy
        /// </summary>
        IJavascriptObjectBuilderStrategy IJavascriptObjectBuilderStrategyFactory.GetStrategy(IWebView webView, ISessionCache cache, bool mapping)
        {
            return webView.AllowBulkCreation ? (IJavascriptObjectBuilderStrategy)new JavascriptObjectMixtBuilderStrategy(webView, cache, mapping) :
                                             new JavascriptObjectSynchroneousBuilderStrategy(webView, cache, mapping);
        }
    }

}
