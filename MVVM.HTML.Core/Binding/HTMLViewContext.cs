using System.Threading.Tasks;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
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

        public IJavascriptSessionInjector JavascriptSessionInjector { get; private set; }

        private IJavascriptSessionInjectorFactory JavascriptSessionInjectorFactory { get; set; }

        public IJavascriptSessionInjector CreateInjector(IJavascriptChangesObserver JavascriptObjecChanges)
        {
            return JavascriptSessionInjector = JavascriptSessionInjectorFactory.CreateInjector(WebView, JavascriptObjecChanges);
        }

        internal async Task<BidirectionalMapper> GetMapper(object viewModel, JavascriptBindingMode iMode, object additional)
        {
            var mapper = await WebView.EvaluateAsync(() => new BidirectionalMapper(viewModel, this, iMode, additional));
            await mapper.Init();
            return mapper;
        }
    }
}
