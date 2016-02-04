using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Window;

namespace MVVM.HTML.Core.Binding
{
    public class HTMLViewContext
    {
        public HTMLViewContext(IWebView webView, IDispatcher uiDispatcher, IJavascriptSessionInjectorFactory javascriptSessionInjectorFactory)
        {
            WebView = webView;
            UIDispatcher = uiDispatcher;
            JavascriptSessionInjectorFactory = javascriptSessionInjectorFactory;
        }
        public IWebView WebView { get; private set; }

        public IDispatcher UIDispatcher { get; private set; }

        private IJavascriptSessionInjectorFactory JavascriptSessionInjectorFactory { get; set; }

        public IJavascriptSessionInjector CreateInjector(IJavascriptChangesListener JavascriptObjecChanges)
        {
            return JavascriptSessionInjectorFactory.CreateInjector(WebView, JavascriptObjecChanges);
        }
    }
}
