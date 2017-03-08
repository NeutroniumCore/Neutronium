using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;
using MoreCollection.Extensions;

namespace Neutronium.JavascriptFramework.Vue.Communication
{
    public class WebViewCommunication : IWebViewCommunication
    {
        private readonly Dictionary<IWebView, WebViewListener> _Listeners = new Dictionary<IWebView, WebViewListener>();

        public void RegisterCommunicator(IWebView webView)
        {
            Get(webView);
        }

        private WebViewListener Get(IWebView webView)
        {
            return _Listeners.GetOrAddEntity(webView, RegisterWebView);
        }

        private static WebViewListener RegisterWebView(IWebView webView)
        {
            return new WebViewListener(webView);
        }

        public IDisposable Subscribe(IWebView webView, Action<string> onEvent)
        {
            var listener = Get(webView);
            return listener.Subscribe(onEvent);
        }

        public IDisposable Listen(IWebView source, IWebView target)
        {
            Action<string> dispatch = message => target.ExecuteJavaScript($"window.postMessage({message},'*');");
            return Subscribe(source, dispatch);
        }
    }
}
