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

        public IDisposable Subscribe(IWebView webView, string channel, Action<string> onEvent)
        {
            var listener = Get(webView);
            return listener.Subscribe(channel, onEvent);
        }

        public IDisposable SubscribeAll(IWebView webView, Action<string, string> onEvent)
        {
            var listener = Get(webView);
            return listener.SubscribeAllChannel(onEvent);
        }

        public IDisposable ExecuteCodeOnAllEvents(IWebView source, IWebView target, Func<string,string, string> codeBuilder) 
        {
            return SubscribeAll(source, GetDispatchAction(target, codeBuilder));
        }

        private static Action<string, string> GetDispatchAction(IWebView target, Func<string, string, string> codeBuilder) 
        {
            return (channel, message) =>
            {
                var transformed = message.Replace(@"\", @"\\");
                target.ExecuteJavaScript(codeBuilder($"'{channel}'", $"'{transformed}'"));
            };
        }
    }
}
