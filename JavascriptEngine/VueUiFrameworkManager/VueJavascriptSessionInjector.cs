using MVVM.HTML.Core.JavascriptUIFramework;
using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Exceptions;

namespace VueUiFramework
{
    internal class VueJavascriptSessionInjector : IJavascriptSessionInjector
    {
        private IWebView _WebView;
        private IJavascriptChangesObserver _JavascriptObserver;
        private IJavascriptObject _VueHelper;

        public VueJavascriptSessionInjector(IWebView webView, IJavascriptChangesObserver javascriptObserver)
        {
            _WebView = webView;
            _JavascriptObserver = javascriptObserver;
        }

        public IJavascriptObject Inject(IJavascriptObject rawObject, IJavascriptObjectMapper mapper)
        {
            _WebView.RunAsync(mapper.AutoMap);   
            return rawObject;
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

        public Task RegisterMainViewModel(IJavascriptObject jsObject)
        {
             return _WebView.RunAsync(() => UnsafeInject(jsObject));
        }

        public IJavascriptObject UnsafeInject(IJavascriptObject ijvm)
        {
            var res = GetVueHelper().Invoke("register", _WebView, ijvm);
            return (res == null || res.IsUndefined) ? null : res;
        }

        public void Dispose()
        {
        }
    }
}
