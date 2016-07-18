using System;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace VueUiFramework 
{
    public class VueVmManager : IJavascriptViewModelManager 
    {
        public IJavascriptSessionInjector Injector => _VueJavascriptSessionInjector;
        public IJavascriptViewModelUpdater ViewModelUpdater => _VueJavascriptViewModelUpdater;

        private readonly IWebView _WebView;
        private readonly Lazy<IJavascriptObject> _VueHelperLazy;
        private readonly VueJavascriptSessionInjector _VueJavascriptSessionInjector;
        private readonly VueJavascriptViewModelUpdater _VueJavascriptViewModelUpdater;

        public VueVmManager(IWebView webView, IJavascriptObject listener) 
        {
            _WebView = webView;
            _VueHelperLazy = new Lazy<IJavascriptObject>(GetVueHelper);
            _VueJavascriptSessionInjector =  new VueJavascriptSessionInjector(webView, listener, _VueHelperLazy);
            _VueJavascriptViewModelUpdater = new VueJavascriptViewModelUpdater(webView, listener, _VueHelperLazy);
        }

        private IJavascriptObject GetVueHelper() 
        {
            var vueHelper = _WebView.GetGlobal().GetValue("glueHelper");
            if ((vueHelper == null) || (!vueHelper.IsObject))
                throw ExceptionHelper.Get("glueHelper not found!");

            return vueHelper;
        }

        public void Dispose() 
        {
            _VueJavascriptSessionInjector.Dispose();
            _VueJavascriptViewModelUpdater.Dispose();
        }
    }
}
