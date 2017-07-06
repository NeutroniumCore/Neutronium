using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectBuilder
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;
  
        public JavascriptObjectBuilder(IWebView webView, IJavascriptSessionCache cache)
        {
            _WebView = webView;
            _Cache = cache;
        }  
  
        public void UpdateJavascriptValue(IJSCSGlue root)
        {
            var builder = new JavascriptObjectOneShotBuilder(_WebView, _Cache, root);
            builder.UpdateJavascriptValue();
        }
    }
}
