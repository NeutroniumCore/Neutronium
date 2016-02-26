using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core.Binding
{
    public class HTMLViewEngine
    {
        private IHTMLWindowProvider _HTMLWindowProvider;
        private IJavascriptUIFrameworkManager _uiFrameworkManager;

        internal HTMLViewEngine(IHTMLWindowProvider hTMLWindowProvider, IJavascriptUIFrameworkManager uiFrameworkManager)
        {
            _HTMLWindowProvider = hTMLWindowProvider;
            _uiFrameworkManager = uiFrameworkManager;
        }

        private IWebView MainView
        {
            get { return _HTMLWindowProvider.HTMLWindow.MainFrame; }
        }

        public HTMLViewContext GetMainContext(IJavascriptChangesObserver javascriptChangesObserver)
        {
            return new HTMLViewContext(MainView, _HTMLWindowProvider.UIDispatcher, _uiFrameworkManager, javascriptChangesObserver);
        }

        internal async Task<BidirectionalMapper> GetMapper(object viewModel, JavascriptBindingMode iMode, object additional)
        {
            var mapper = await MainView.EvaluateAsync(() => new BidirectionalMapper(viewModel, this, iMode, additional));
            await mapper.Init();
            return mapper;
        }
    }
}
