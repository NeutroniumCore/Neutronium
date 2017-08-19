//using System;
//using Neutronium.Core.WebBrowserEngine.JavascriptObject;

//namespace Neutronium.Core.Extension
//{
//    public static class IJavascriptObjectExtension
//    {
//        public static void Bind(this IJavascriptObject javascriptObject, string functionName, IWebView webView, Action<IJavascriptObject[]> action)
//        {
//            Action<string, IJavascriptObject, IJavascriptObject[]> neededAction = (_, __, arg) => action(arg);
//            javascriptObject.Bind(functionName, webView, neededAction);
//        }
//    }
//}
