using System;
using MVVM.HTML.Core.JavascriptUIFramework;
using System.Threading.Tasks;
using MVVM.HTML.Core;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace VueUiFramework
{
    internal class VueJavascriptSessionInjector : IJavascriptSessionInjector
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
        private readonly Lazy<IJavascriptObject> _VueHelper;
        private readonly IWebSessionLogger _Logger;
        private IJavascriptObject _ReadyListener;

        public VueJavascriptSessionInjector(IWebView webView, IJavascriptObject listener, Lazy<IJavascriptObject> vueHelper, IWebSessionLogger logger)
        {
            _WebView = webView;
            _Listener = listener;
            _VueHelper = vueHelper;
            _Logger = logger;
        }

        public IJavascriptObject Inject(IJavascriptObject rawObject, IJavascriptObjectMapper mapper)
        {
            mapper?.AutoMap();   
            return rawObject;
        }

        public async Task RegisterMainViewModel(IJavascriptObject jsObject)
        {
             await await _WebView.EvaluateAsync(() => UnsafeRegister(jsObject));
        }

        private Task UnsafeRegister(IJavascriptObject ijvm)
        {
            var tcs = new TaskCompletionSource<object>();
            _ReadyListener = _WebView.Factory.CreateObject(false);
            _ReadyListener.Bind("fulfill", _WebView, (_, __, ___) => 
            {
                _Logger.Debug("Vue ready received");
                tcs.TrySetResult(null);
            });

            var vueHelper = _VueHelper.Value;
            vueHelper.GetValue("ready").Invoke("then", _WebView, _ReadyListener.GetValue("fulfill"));
            vueHelper.Invoke("register", _WebView, ijvm, _Listener);
            return tcs.Task;
        }
    }
}
