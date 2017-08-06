using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    public interface IJavascriptObjectBuilderStrategyFactory
    {
        IJavascriptObjectBuilderStrategy GetStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping);
    }
}