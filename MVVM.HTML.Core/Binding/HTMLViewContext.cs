using System;
using System.Threading.Tasks;
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
        private IJavascriptUIFrameworkManager JavascriptUiFrameworkManager { get; set; }

        public HTMLViewContext(IWebView webView, IDispatcher uiDispatcher, IJavascriptUIFrameworkManager javascriptUiFrameworkManager)
        {
            WebView = webView;
            UIDispatcher = uiDispatcher;
            JavascriptUiFrameworkManager = javascriptUiFrameworkManager;
            ViewModelUpdater = javascriptUiFrameworkManager.CreateViewModelUpdater(WebView);
        }

        public IJavascriptSessionInjector CreateInjector(IJavascriptChangesObserver JavascriptObjecChanges)
        {
            return JavascriptSessionInjector = JavascriptUiFrameworkManager.CreateInjector(WebView, JavascriptObjecChanges);
        }

        internal async Task<BidirectionalMapper> GetMapper(object viewModel, JavascriptBindingMode iMode, object additional)
        {
            var mapper = await WebView.EvaluateAsync(() => new BidirectionalMapper(viewModel, this, iMode, additional));
            await mapper.Init();
            return mapper;
        }

        public void Dispose()
        {
            JavascriptSessionInjector.Dispose();
            ViewModelUpdater.Dispose();
        }
    }
}
