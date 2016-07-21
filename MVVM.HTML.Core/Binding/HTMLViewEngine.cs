using System;
using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core.Binding
{
    public class HTMLViewEngine
    {
        private readonly IHTMLWindowProvider _HTMLWindowProvider;
        private readonly IJavascriptUIFrameworkManager _UIFrameworkManager;

        public HTMLViewEngine(IHTMLWindowProvider hTMLWindowProvider, IJavascriptUIFrameworkManager uiFrameworkManager)
        {
            _HTMLWindowProvider = hTMLWindowProvider;
            _UIFrameworkManager = uiFrameworkManager;
        }

        private IWebView MainView => _HTMLWindowProvider.HTMLWindow.MainFrame;

        public HTMLViewContext GetMainContext(IJavascriptChangesObserver javascriptChangesObserver)
        {
            return new HTMLViewContext(MainView, _HTMLWindowProvider.UIDispatcher, _UIFrameworkManager, javascriptChangesObserver);
        }

        internal async Task<BidirectionalMapper> GetMapper(object viewModel, JavascriptBindingMode mode, object additional)
        {
            var mapper = await MainView.EvaluateAsync(() => Init(viewModel, mode, additional));
            await mapper.Item2;
            return mapper.Item1;
        }

        private Tuple<BidirectionalMapper,Task> Init(object viewModel, JavascriptBindingMode mode, object additional) 
        {
            var res = new BidirectionalMapper(viewModel, this, mode, additional);
            var task = res.Init();
            return new Tuple<BidirectionalMapper, Task>(res, task);
        }
    }
}
