using System;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core.Binding
{
    public class HTMLViewContext : IDisposable
    {
        public IWebView WebView { get; private set; }
        public IDispatcher UIDispatcher { get; private set; }
        public IJavascriptSessionInjector JavascriptSessionInjector { get; private set; }
        public IJavascriptViewModelUpdater ViewModelUpdater { get; private set; }

        public HTMLViewContext(IWebView webView, IDispatcher uiDispatcher, IJavascriptUIFrameworkManager javascriptUiFrameworkManager,
                                IJavascriptChangesObserver javascriptChangesObserver)
        {
            WebView = webView;
            UIDispatcher = uiDispatcher;
            ViewModelUpdater = javascriptUiFrameworkManager.CreateViewModelUpdater(WebView);
            JavascriptSessionInjector = javascriptUiFrameworkManager.CreateInjector(WebView, javascriptChangesObserver);
        }

        public void Dispose()
        {
            JavascriptSessionInjector.Dispose();
            ViewModelUpdater.Dispose();
        }
    }
}
