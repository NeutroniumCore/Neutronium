using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Extension;
using System;
using Neutronium.Core.Infra;

namespace Neutronium.JavascriptFramework.Vue.Communication
{
    internal class WebViewListener : IDisposable
    {
        private EventHandler<MessageEvent> _MessageReceived;
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
         
        public WebViewListener(IWebView webView)
        {
            _WebView = webView;

            _Listener = _WebView.Factory.CreateObject(false);
            _Listener.Bind("postMessage", _WebView, (e) => PostMessage(e[0].GetStringValue(), e[1].GetStringValue()));
            _WebView.GetGlobal().SetValue("__listener__", _Listener, CreationOption.DontDelete | CreationOption.ReadOnly | CreationOption.DontEnum);
        }

        private void PostMessage(string channel, string message)
        {
            _MessageReceived?.Invoke(this, new MessageEvent(channel, message));
        }

        public IDisposable Subscribe(string channel, Action<string> onEvent)
        {
            EventHandler<MessageEvent> ea = (o, message) =>
            {
                if (channel==message.Channel)
                    onEvent(message.Message);
            };
            _MessageReceived += ea;
            return new DisposableAction(() => _MessageReceived -= ea);
        }

        public void Dispose()
        {
            _Listener.Dispose();
        }
    }
}
