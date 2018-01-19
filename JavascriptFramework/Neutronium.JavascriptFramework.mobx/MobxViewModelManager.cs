using System;
using Neutronium.Core;
using Neutronium.Core.Exceptions;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.JavascriptFramework.mobx 
{
    public class MobxViewModelManager : IJavascriptViewModelManager 
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
        private readonly IWebSessionLogger _Logger;
        private readonly Lazy<IJavascriptObject> _MobxHelperLazy;

        public IJavascriptSessionInjector Injector { get; } 
        public IJavascriptViewModelUpdater ViewModelUpdater { get; }

        public MobxViewModelManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger) 
        {
            _WebView = webView;
            _Listener = listener;
            _Logger = logger;
            _MobxHelperLazy = new Lazy<IJavascriptObject>(GetMoxHelper);
            Injector = new MobxJavascriptSessionInjector(webView, _MobxHelperLazy, listener, logger);
            ViewModelUpdater = new MobxJavascriptViewModelUpdater(webView, _MobxHelperLazy, listener, logger);
        }

        private IJavascriptObject GetMoxHelper() 
        {
            var mobxHelper = _WebView.GetGlobal().GetValue("mobxManager");
            if ((mobxHelper == null) || (mobxHelper.IsUndefined))
                throw ExceptionHelper.Get("mobxHelper not found!");

            return mobxHelper;
        }

        public void Dispose()
        {
            if (_MobxHelperLazy.IsValueCreated)
                _MobxHelperLazy.Value.Dispose();
        }
    }
}
