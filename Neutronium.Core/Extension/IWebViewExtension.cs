using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;

namespace Neutronium.Core.Extension
{
    public static class IWebViewExtension
    {
        public static IEnumerable<IJavascriptObject[]> Slice(this IWebView webView, IList<IJavascriptObject> elements)
        {
            var slicer = new Slicer<IJavascriptObject>(elements, webView.MaxFunctionArgumentsNumber);
            return slicer.Slice();
        }
    }
}
