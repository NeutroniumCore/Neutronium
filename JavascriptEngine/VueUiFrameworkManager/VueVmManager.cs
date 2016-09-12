using System;
using Neutronium.Core;
using Neutronium.Core.Exceptions;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptUIFramework;

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
        private readonly IWebSessionLogger _Logger;

        public VueVmManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger) 
        {
            _WebView = webView;
            _Logger = logger;
            _VueHelperLazy = new Lazy<IJavascriptObject>(GetVueHelper);
            _VueJavascriptSessionInjector =  new VueJavascriptSessionInjector(webView, listener, _VueHelperLazy, _Logger);
            _VueJavascriptViewModelUpdater = new VueJavascriptViewModelUpdater(webView, listener, _VueHelperLazy, _Logger);    
        }

        private IJavascriptObject GetVueHelper() 
        {
            var vueHelper = _WebView.GetGlobal().GetValue("glueHelper");
            if ((vueHelper == null) || (vueHelper.IsUndefined))
                throw ExceptionHelper.Get("glueHelper not found!");

            return vueHelper;
        }

        public void Dispose() 
        {
            if (_VueHelperLazy.IsValueCreated)
                _VueHelperLazy.Value.Dispose();

            _VueJavascriptViewModelUpdater.Dispose();
            _Logger.Debug("VueVmManager disposed");
        }
    }
}
