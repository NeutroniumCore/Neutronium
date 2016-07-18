using System;
using MVVM.HTML.Core.JavascriptUIFramework;
using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace VueUiFramework
{
    internal class VueJavascriptSessionInjector : IJavascriptSessionInjector
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
        private readonly Lazy<IJavascriptObject> _VueHelper;

        public VueJavascriptSessionInjector(IWebView webView, IJavascriptObject listener, Lazy<IJavascriptObject> vueHelper)
        {
            _WebView = webView;
            _Listener = listener;
            _VueHelper = vueHelper;
        }

        public IJavascriptObject Inject(IJavascriptObject rawObject, IJavascriptObjectMapper mapper)
        {
            _WebView.Run(()=> UnsafeInject(rawObject));
            _WebView.RunAsync(mapper.AutoMap);   
            return rawObject;
        }

        private IJavascriptObject UnsafeInject(IJavascriptObject rawObject) 
        {
            return rawObject;
        }

        public Task RegisterMainViewModel(IJavascriptObject jsObject)
        {
             return _WebView.RunAsync(() => UnsafeRegister(jsObject));
        }

        private IJavascriptObject UnsafeRegister(IJavascriptObject ijvm)
        {
            var res = _VueHelper.Value.Invoke("register", _WebView, ijvm, _Listener);
            return (res == null || res.IsUndefined) ? null : res;
        }

        public void Dispose()
        {
        }
    }
}
