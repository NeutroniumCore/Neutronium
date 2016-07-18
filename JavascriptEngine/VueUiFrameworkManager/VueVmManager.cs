using System;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace VueUiFramework {
    public class VueVmManager : IJavascriptViewModelManager {
        public IJavascriptSessionInjector Injector { get; }
        public IJavascriptViewModelUpdater ViewModelUpdater { get; }

        private readonly IWebView _WebView;
        private readonly Lazy<IJavascriptObject> _VueHelperLazy;

        public VueVmManager(IWebView webView, IJavascriptObject listener) 
        {
            _WebView = webView;
            _VueHelperLazy = new Lazy<IJavascriptObject>(GetVueHelper);
            Injector =  new VueJavascriptSessionInjector(webView, listener, _VueHelperLazy);
            ViewModelUpdater = new VueJavascriptViewModelUpdater(webView, listener, _VueHelperLazy);
        }

        private IJavascriptObject GetVueHelper() 
        {
            var vueHelper = _WebView.GetGlobal().GetValue("glueHelper");
            if ((vueHelper == null) || (!vueHelper.IsObject))
                throw ExceptionHelper.Get("glueHelper not found!");

            return vueHelper;
        }
    }
}
