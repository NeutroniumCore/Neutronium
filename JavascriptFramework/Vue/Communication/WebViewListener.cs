using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using Neutronium.Core.Infra;
using System.Threading.Tasks;

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

            _Listener = _WebView.Factory.CreateObject();
            _Listener.BindArguments("postMessage", _WebView, (chanel, message) => PostMessage(chanel.GetStringValue(), message.GetStringValue()));
            _WebView.GetGlobal().SetValue("__neutronium_listener__", _Listener, CreationOption.DontDelete | CreationOption.ReadOnly | CreationOption.DontEnum);

            var loader = new ResourceReader("Communication.script.dist", this);
            var data = loader.Load("communication.js");
            _WebView.ExecuteJavaScript(data);
        }

        private void PostMessage(string channel, string message)
        {
            _MessageReceived?.Invoke(this, new MessageEvent(channel, message));
        }

        public IDisposable Subscribe(string channel, Action<string> onEvent)
        {
            EventHandler<MessageEvent> ea = (o, message) => Task.Run(() =>
            {
                if (channel == message.Channel)
                    onEvent(message.Message);
            });       
            return Register(ea);
        }

        public IDisposable SubscribeAllChannel(Action<string, string> onEvent)
        {
            EventHandler<MessageEvent> ea = (o, message) => Task.Run(() =>
            {
                onEvent(message.Channel , message.Message);
            });
            return Register(ea);
        }

        private IDisposable Register(EventHandler<MessageEvent> eventHandler)
        {
            _MessageReceived += eventHandler;
            return new DisposableAction(() => _MessageReceived -= eventHandler);
        }

        public void Dispose()
        {
            _Listener.Dispose();
        }
    }
}
