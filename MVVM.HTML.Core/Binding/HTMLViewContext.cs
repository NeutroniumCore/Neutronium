using System;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptEngine.Window;
using Neutronium.Core.JavascriptFramework;

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

        public HTMLViewContext(IWebView webView, IDispatcher uiDispatcher, IJavascriptUiFrameworkManager javascriptUiFrameworkManager,
                                IJavascriptChangesObserver javascriptChangesObserver, IWebSessionLogger logger)
        {
            WebView = webView;
            _logger = logger;
            UIDispatcher = uiDispatcher;
            var builder = new BinderBuilder(webView, javascriptChangesObserver);
            _Listener = builder.BuildListener();
            _VmManager = javascriptUiFrameworkManager.CreateManager(WebView, _Listener, _logger);
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
