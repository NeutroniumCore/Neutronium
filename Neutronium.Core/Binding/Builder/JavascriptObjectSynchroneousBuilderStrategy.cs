using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectSynchroneousBuilderStrategy : IJavascriptObjectBuilderStrategy
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;

        public JavascriptObjectSynchroneousBuilderStrategy(IWebView webView, IJavascriptSessionCache cache)
        {
            _WebView = webView;
            _Cache = cache;
        }

        public void UpdateJavascriptValue(IJSCSGlue root)
        {
            var builder = new JavascriptObjectSynchroneousBuilder(_WebView.Factory, _Cache, root);
            builder.UpdateJavascriptValue();
        }
    }
}
