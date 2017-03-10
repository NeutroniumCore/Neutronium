using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            return listener.Subscribe(channel, message => Task.Run(() => onEvent(message)));
        }

        public IDisposable ExecuteCodeOnEvent(IWebView source, string channel, IWebView target, Func<string,string> codeBuilder) 
        {
            return Subscribe(source, channel, GetDispatchAction(target, codeBuilder));
        }

        private static Action<string> GetDispatchAction(IWebView target, Func<string, string> codeBuilder) 
        {
            return message =>
            {
                var transformed = message.Replace(@"\", @"\\");
                target.ExecuteJavaScript(codeBuilder($"'{transformed}'"));
            };
        }
    }
}
