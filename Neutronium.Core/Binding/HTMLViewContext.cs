using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding
{
    public class HTMLViewContext : IDisposable
    {
        public IWebView WebView { get; }
        public IDispatcher UIDispatcher { get; }
        public IJavascriptSessionInjector JavascriptSessionInjector { get; }
        public IJavascriptViewModelUpdater ViewModelUpdater { get; }
        private readonly IJavascriptObject _Listener;
        private readonly IJavascriptViewModelManager _VmManager;
        private readonly IWebSessionLogger _logger;

        public HTMLViewContext(IWebView webView, IDispatcher uiDispatcher, IJavascriptFrameworkManager javascriptFrameworkManager,
                                IJavascriptChangesObserver javascriptChangesObserver, IWebSessionLogger logger)
        {
            WebView = webView;
            _logger = logger;
            UIDispatcher = uiDispatcher;
            var builder = new BinderBuilder(webView, javascriptChangesObserver);
            _Listener = builder.BuildListener();
            _VmManager = javascriptFrameworkManager.CreateManager(WebView, _Listener, _logger);
            ViewModelUpdater = _VmManager.ViewModelUpdater;
            JavascriptSessionInjector = _VmManager.Injector;
        }

        public void Dispose() 
        {
            _VmManager.Dispose();
            _Listener.Dispose();
            _logger.Debug("HTMLViewContext Disposed");
        }
    }
}
