using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectMixtBuilderStrategy : IJavascriptObjectBuilderStrategy
    {
        private readonly IJavascriptObjectBuilderStrategy _SynchroneousStrategy;
        private readonly IJavascriptObjectBuilderStrategy _BulkStrategy;

        public JavascriptObjectMixtBuilderStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping)
        {
            _SynchroneousStrategy = new JavascriptObjectSynchroneousBuilderStrategy(webView, cache, mapping);
            _BulkStrategy = new JavascriptObjectBulkBuilderStrategy(webView, cache, mapping);
        }

        private IJavascriptObjectBuilderStrategy GetStrategy(IJsCsGlue root) => root.IsBasic()? _SynchroneousStrategy : _BulkStrategy;

        public void UpdateJavascriptValue(IJsCsGlue root)
        {
            GetStrategy(root).UpdateJavascriptValue(root);
        }

        public void Dispose()
        {
            _SynchroneousStrategy.Dispose();
            _BulkStrategy.Dispose();
        }
    }
}
