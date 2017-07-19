using System;
using Neutronium.Core;
using Neutronium.Core.Exceptions;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.JavascriptFramework.Vue.Communication;

namespace Neutronium.JavascriptFramework.Vue 
{
    public class VueVmManager : IJavascriptViewModelManager 
    {
        public IJavascriptSessionInjector Injector => _VueJavascriptSessionInjector;
        public IJavascriptViewModelUpdater ViewModelUpdater => _VueJavascriptViewModelUpdater;

        private readonly IWebView _WebView;
        private readonly Lazy<IJavascriptObject> _VueHelperLazy;
        private readonly VueJavascriptSessionInjector _VueJavascriptSessionInjector;
        private readonly VueJavascriptViewModelUpdater _VueJavascriptViewModelUpdater;
        private readonly IWebViewCommunication _WebViewCommunication;
        private readonly IWebSessionLogger _Logger;

        public VueVmManager(IWebView webView, IJavascriptObject listener, IWebViewCommunication webViewCommunication, IWebSessionLogger logger) 
        {
            _WebView = webView;
            _Logger = logger;
            _WebViewCommunication = webViewCommunication;
            _VueHelperLazy = new Lazy<IJavascriptObject>(GetVueHelper);
            _VueJavascriptSessionInjector =  new VueJavascriptSessionInjector(webView, listener, _VueHelperLazy, _Logger);
            _VueJavascriptViewModelUpdater = new VueJavascriptViewModelUpdater(webView, listener, _VueHelperLazy, _Logger);    
        }

        private IJavascriptObject GetVueHelper() 
        {
            var vueHelper = _WebView.GetGlobal().GetValue("glueHelper");
            if ((vueHelper == null) || (vueHelper.IsUndefined))
                throw ExceptionHelper.Get("glueHelper not found!");

            _WebViewCommunication?.RegisterCommunicator(_WebView);
            return vueHelper;
        }

        public void Dispose() 
        {
            if (_VueHelperLazy.IsValueCreated)
                _VueHelperLazy.Value.Dispose();

            _WebViewCommunication?.Disconnect(_WebView);
            _Logger.Debug("VueVmManager disposed");
        }
    }
}
