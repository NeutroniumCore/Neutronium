using System;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;

namespace Neutronium.Core.Extension
{
    public static class WebViewExtension
    {
        public static IEnumerable<IJavascriptObject[]> Slice(this IWebView webView, IEnumerable<IJavascriptObject> elements)
        {
            var slicer = new Slicer<IJavascriptObject>(elements, webView.GetMaxAcceptableArguments());
            return slicer.Slice();
        }

        public static int GetMaxAcceptableArguments(this IWebView webView)
        {
            return Math.Min(webView.MaxFunctionArgumentsNumber, ClrRuntime.LohArraySize);
        } 
    }
}
