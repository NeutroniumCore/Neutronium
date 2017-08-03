using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectSynchroneousBuilderStrategy : IJavascriptObjectBuilderStrategy
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;
        private readonly bool _Mapping;

        public JavascriptObjectSynchroneousBuilderStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping)
        {
            _WebView = webView;
            _Cache = cache;
            _Mapping = mapping;
        }

        public void UpdateJavascriptValue(IJSCSGlue root)
        {
            var builder = new JavascriptObjectSynchroneousBuilder(_WebView.Factory, _Cache, root, _Mapping);
            builder.UpdateJavascriptValue();
        }
    }
}
