using System;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core.Binding
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

        public HTMLViewContext(IWebView webView, IDispatcher uiDispatcher, IJavascriptUIFrameworkManager javascriptUiFrameworkManager,
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
        }
    }
}
