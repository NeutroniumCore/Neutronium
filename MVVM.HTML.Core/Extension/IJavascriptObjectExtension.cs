using System;
using Neutronium.Core.JavascriptEngine.JavascriptObject;

namespace Neutronium.Core.Extension
{
    public static class IJavascriptObjectExtension
    {
        public static void Bind(this IJavascriptObject javascriptObject, string functionName, IWebView webView, Action<IJavascriptObject[]> action)
        {
            Action<string, IJavascriptObject, IJavascriptObject[]> neededAction = (n, t, arg) => action(arg);
            javascriptObject.Bind(functionName, webView, neededAction);
        }
    }
}
