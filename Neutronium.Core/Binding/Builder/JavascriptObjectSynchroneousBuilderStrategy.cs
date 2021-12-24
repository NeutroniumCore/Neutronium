using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    public class JavascriptObjectSynchroneousBuilderStrategy : IJavascriptObjectBuilderStrategy
    {
        private readonly IWebView _WebView;
        private readonly ISessionCache _Cache;
        private readonly bool _Mapping;

        public JavascriptObjectSynchroneousBuilderStrategy(IWebView webView, ISessionCache cache, bool mapping)
        {
            _WebView = webView;
            _Cache = cache;
            _Mapping = mapping;
        }

        public void UpdateJavascriptValue(IJsCsGlue root)
        {
            var builder = new JavascriptObjectSynchroneousBuilder(_WebView.Factory, _Cache, root, _Mapping);
            builder.UpdateJavascriptValue();
        }

        public void Dispose()
        {
        }
    }
}
