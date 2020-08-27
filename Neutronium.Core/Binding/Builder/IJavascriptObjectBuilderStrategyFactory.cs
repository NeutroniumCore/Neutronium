using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    internal interface IJavascriptObjectBuilderStrategyFactory
    {
        IJavascriptObjectBuilderStrategy GetStrategy(IWebView webView, ISessionCache cache, bool mapping);
    }
}