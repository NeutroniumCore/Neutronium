using MVVM.HTML.Core.JavascriptUIFramework;
using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Exceptions;

namespace VueUiFramework
{
    internal class VueJavascriptSessionInjector : IJavascriptSessionInjector
    {
        private readonly IWebView _WebView;
        private IJavascriptObject _VueHelper;
        private readonly IJavascriptObject _Listener;

        public VueJavascriptSessionInjector(IWebView webView, IJavascriptObject listener)
        {
            _WebView = webView;
            _Listener = listener;
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
            var res = GetVueHelper().Invoke("register", _WebView, ijvm, _Listener);
            return (res == null || res.IsUndefined) ? null : res;
        }

        private IJavascriptObject GetVueHelper()
        {
            if (_VueHelper != null)
                return _VueHelper;

            _VueHelper = _WebView.GetGlobal().GetValue("glueHelper");
            if ((_VueHelper == null) || (!_VueHelper.IsObject))
                throw ExceptionHelper.Get("glueHelper not found!");

            return _VueHelper;
        }

        public void Dispose()
        {
        }
    }
}
