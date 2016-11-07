using System;
using System.Threading.Tasks;
using Neutronium.Core;
using Neutronium.Core.Extension;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.JavascriptFramework.Vue
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
            var tcs = new TaskCompletionSource<object>();
            _Listener.Bind("fulfill", _WebView, _ => 
            {
                _Logger.Debug("Vue ready received");
                tcs.TrySetResult(null);
            });

            var vueHelper = _VueHelper.Value;
            vueHelper.GetValue("ready").Invoke("then", _WebView, _Listener.GetValue("fulfill"));       
            vueHelper.Invoke("register", _WebView, jsObject, _Listener);
            return tcs.Task;
        }
    }
}
