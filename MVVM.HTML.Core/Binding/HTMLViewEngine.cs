using System;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core.Binding
{
    public class HTMLViewEngine
    {
        private readonly IHTMLWindowProvider _HTMLWindowProvider;
        private readonly IJavascriptUIFrameworkManager _UIFrameworkManager;
        public IWebSessionLogger Logger { get; }

        private IWebView MainView => _HTMLWindowProvider.HTMLWindow.MainFrame;

        public HTMLViewEngine(IHTMLWindowProvider hTMLWindowProvider, IJavascriptUIFrameworkManager uiFrameworkManager, IWebSessionLogger logger)
        {
            _HTMLWindowProvider = hTMLWindowProvider;
            _UIFrameworkManager = uiFrameworkManager;
            Logger = logger;
        }

        public HTMLViewContext GetMainContext(IJavascriptChangesObserver javascriptChangesObserver)
        {
            return new HTMLViewContext(MainView, _HTMLWindowProvider.UIDispatcher, _UIFrameworkManager, javascriptChangesObserver, Logger);
        }

        internal BidirectionalMapper GetMapper(object viewModel, JavascriptBindingMode mode, object additional)
        {
            return Evaluate(() => new BidirectionalMapper(viewModel, this, mode, additional, Logger));
        }

        public T Evaluate<T>(Func<T> compute)
        {
            return MainView.Evaluate(compute);
        }
    }
}
