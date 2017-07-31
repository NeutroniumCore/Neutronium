using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectSynchroneousBuilderStrategy : IJavascriptObjectBuilderStrategy
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;
        private readonly bool _NeedToCacheObject;

        public JavascriptObjectSynchroneousBuilderStrategy(IWebView webView, IJavascriptSessionCache cache, bool needToCacheObject)
        {
            _WebView = webView;
            _Cache = cache;
            _NeedToCacheObject = needToCacheObject;
        }

        public void UpdateJavascriptValue(IJSCSGlue root)
        {
            var builder = new JavascriptObjectSynchroneousBuilder(_WebView.Factory, _Cache, root, _NeedToCacheObject);
            builder.UpdateJavascriptValue();
        }
    }
}
