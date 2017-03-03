using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding
{
    public class HTMLViewEngine
    {
        private readonly IWebBrowserWindowProvider _HTMLWindowProvider;
        private readonly IJavascriptFrameworkManager _frameworkManager;
        public IWebSessionLogger Logger { get; }

        private IWebView MainView => HTMLWindow.MainFrame;
        public IWebBrowserWindow HTMLWindow => _HTMLWindowProvider.HTMLWindow;

        public HTMLViewEngine(IWebBrowserWindowProvider hTMLWindowProvider, IJavascriptFrameworkManager frameworkManager, IWebSessionLogger logger)
        {
            _HTMLWindowProvider = hTMLWindowProvider;
            _frameworkManager = frameworkManager;
            Logger = logger;
        }

        public HTMLViewContext GetMainContext(IJavascriptChangesObserver javascriptChangesObserver)
        {
            return new HTMLViewContext(HTMLWindow, _HTMLWindowProvider.UIDispatcher, _frameworkManager, javascriptChangesObserver, Logger);
        }

        internal BidirectionalMapper GetMapper(object viewModel, JavascriptBindingMode mode)
        {
            return new BidirectionalMapper(viewModel, this, mode, Logger);
        }

        public T Evaluate<T>(Func<T> compute)
        {
            return MainView.Evaluate(compute);
        }
    }
}
