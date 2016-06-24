using MVVM.HTML.Core.JavascriptUIFramework;
using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Extension;
using UIFramework.Uttils;

namespace VueUiFramework
{
    internal class VueJavascriptSessionInjector : IJavascriptSessionInjector
    {
        private IWebView _WebView;
        private IJavascriptObject _VueHelper;
        private IJavascriptObject _Listener;

        public VueJavascriptSessionInjector(IWebView webView, IJavascriptChangesObserver javascriptObserver)
        {
            _WebView = webView;

            var builder = new BinderBuilder(webView, javascriptObserver);
            _Listener = builder.BuildListener();
        }

        public IJavascriptObject Inject(IJavascriptObject rawObject, IJavascriptObjectMapper mapper)
        {
            _WebView.Run(()=> UnsafeInject(rawObject));
            _WebView.RunAsync(mapper.AutoMap);   
            return rawObject;
        }

        private IJavascriptObject UnsafeInject(IJavascriptObject rawObject) 
        {
            return GetVueHelper().Invoke("inject", _WebView, rawObject, _Listener);
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
