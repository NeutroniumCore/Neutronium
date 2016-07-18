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

        public HTMLViewContext(IWebView webView, IDispatcher uiDispatcher, IJavascriptUIFrameworkManager javascriptUiFrameworkManager,
                                IJavascriptChangesObserver javascriptChangesObserver)
        {
            WebView = webView;
            UIDispatcher = uiDispatcher;
            var builder = new BinderBuilder(webView, javascriptChangesObserver);
            _Listener = builder.BuildListener();
            ViewModelUpdater = javascriptUiFrameworkManager.CreateViewModelUpdater(WebView, _Listener);
            JavascriptSessionInjector = javascriptUiFrameworkManager.CreateInjector(WebView, _Listener);
        }

        public void Dispose()
        {
            JavascriptSessionInjector.Dispose();
            ViewModelUpdater.Dispose();
        }
    }
}
