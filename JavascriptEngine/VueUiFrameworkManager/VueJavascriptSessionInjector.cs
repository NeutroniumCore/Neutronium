using System;
using MVVM.HTML.Core.JavascriptUIFramework;
using System.Threading.Tasks;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Extension;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace VueUiFramework
{
    public class VueJavascriptSessionInjector : IJavascriptSessionInjector
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
        private readonly Lazy<IJavascriptObject> _VueHelper;
        private readonly IWebSessionLogger _Logger;
 
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

        public Task RegisterMainViewModel(IJavascriptObject jsObject)
        {
            return _WebView.Evaluate(() => UnsafeRegister(jsObject));
        }

        private Task UnsafeRegister(IJavascriptObject ijvm)
        {
            var tcs = new TaskCompletionSource<object>();
            _Listener.Bind("fulfill", _WebView, _ => 
            {
                _Logger.Debug("Vue ready received");
                tcs.TrySetResult(null);
            });

            var vueHelper = _VueHelper.Value;
            vueHelper.GetValue("ready").Invoke("then", _WebView, _Listener.GetValue("fulfill"));       
            vueHelper.Invoke("register", _WebView, ijvm, _Listener);
            return tcs.Task;
        }
    }
}
