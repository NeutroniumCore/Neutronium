using MoreCollection.Extensions;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;

namespace Neutronium.JavascriptFramework.Vue.Communication
{
    public class WebViewCommunication : IWebViewCommunication
    {
        private readonly Dictionary<IWebView, WebViewListener> _Listeners = new Dictionary<IWebView, WebViewListener>();

        public void RegisterCommunicator(IWebView webView)
        {
            Get(webView);
        }

        public void Disconnect(IWebView webView)
        {
            _Listeners.Remove(webView);
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

        public IDisposable Connect(IWebView first, IWebView second)
        {
            var subscribe1 = SubscribeAll(first, GetDispatchAction(second, PostMessage));
            var subscribe2 = SubscribeAll(second, GetDispatchAction(first, PostMessage));
            return new ComposedDisposable(subscribe1, subscribe2);
        }

        private static string PostMessage(string channel, string message)
        {
            return $"window.__neutronium_listener__.emit({channel},{message});";
        }

        private static Action<string, string> GetDispatchAction(IWebView target, Func<string, string, string> codeBuilder) 
        {
            return (channel, message) =>
            {
                var transformed = JavascriptNamer.GetCreateExpression(message);
                target.ExecuteJavaScript(codeBuilder($"'{channel}'", transformed));
            };
        }
    }
}
