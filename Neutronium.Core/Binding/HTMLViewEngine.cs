using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding
{
    public class HTMLViewEngine
    {
        private readonly IWebBrowserWindowProvider _HTMLWindowProvider;
        private readonly IJavascriptFrameworkManager _frameworkManager;
        public IWebSessionLogger Logger { get; }

        private IWebView MainView => _HTMLWindowProvider.HTMLWindow.MainFrame;

        public HTMLViewEngine(IWebBrowserWindowProvider hTMLWindowProvider, IJavascriptFrameworkManager frameworkManager, IWebSessionLogger logger)
        {
            _HTMLWindowProvider = hTMLWindowProvider;
            _frameworkManager = frameworkManager;
            Logger = logger;
        }

        public HTMLViewContext GetMainContext(IJavascriptChangesObserver javascriptChangesObserver)
        {
            return new HTMLViewContext(MainView, _HTMLWindowProvider.UIDispatcher, _frameworkManager, javascriptChangesObserver, Logger);
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
