using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Extension;
using System;
using Neutronium.Core.Infra;

namespace Neutronium.JavascriptFramework.Vue.Communication
{
    internal class WebViewListener : IDisposable
    {
        private EventHandler<string> _MessageReceived;
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
         
        public WebViewListener(IWebView webView)
        {
            _WebView = webView;

            _Listener = _WebView.Factory.CreateObject(false);
            _Listener.Bind("postMessage", _WebView, (e) => PostMessage(e[0].GetStringValue()));
            _WebView.GetGlobal().SetValue("__listener__", _Listener, CreationOption.DontDelete | CreationOption.ReadOnly | CreationOption.DontEnum);
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

        public void Dispose()
        {
            _Listener.Dispose();
        }
    }
}
