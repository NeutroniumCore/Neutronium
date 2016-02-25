using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core.Binding
{
    public class HTMLViewContext
    {
        public IWebView WebView { get; private set; }
        public IDispatcher UIDispatcher { get; private set; }
        public IJavascriptSessionInjector JavascriptSessionInjector { get; private set; }
        public IJavascriptViewModelUpdater ViewModelUpdater { get { return JavascriptSessionInjector.ViewModelUpdater; } }
        private IJavascriptSessionInjectorFactory JavascriptSessionInjectorFactory { get; set; }

        public HTMLViewContext(IWebView webView, IDispatcher uiDispatcher, IJavascriptSessionInjectorFactory javascriptSessionInjectorFactory)
        {
            WebView = webView;
            UIDispatcher = uiDispatcher;
            JavascriptSessionInjectorFactory = javascriptSessionInjectorFactory;
        }

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
