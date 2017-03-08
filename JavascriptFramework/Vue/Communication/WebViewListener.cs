using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Extension;
using System;
using Neutronium.Core.Infra;

namespace Neutronium.JavascriptFramework.Vue.Communication
{
    internal class WebViewListener
    {
        private EventHandler<string> _MessageReceived;
        private readonly IWebView _WebView;

        public WebViewListener(IWebView webView)
        {
            _WebView = webView;

            var listener = _WebView.Factory.CreateObject(false);
            listener.Bind("postMessage", _WebView, (e) => PostMessage(e[0].GetStringValue()));
            _WebView.GetGlobal().SetValue("__listener__", listener, CreationOption.DontDelete | CreationOption.ReadOnly | CreationOption.DontEnum);
        }

        private void PostMessage(string message)
        {
            _MessageReceived?.Invoke(this, message);
        }

        public IDisposable Subscribe(Action<string> onEvent)
        {
            EventHandler<string> ea = (o, message) => onEvent(message);
            _MessageReceived += ea;
            return new DisposableAction(() => _MessageReceived -= ea);
        }
    }
}
