using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Binding.Builder;

namespace Neutronium.Core.Binding
{
    public class HtmlViewEngine
    {
        private readonly IWebBrowserWindowProvider _HtmlWindowProvider;
        private readonly IJavascriptFrameworkManager _FrameworkManager;
        public IWebSessionLogger Logger { get; }

        private IWebView MainView => HtmlWindow.MainFrame;
        public IWebBrowserWindow HtmlWindow => _HtmlWindowProvider.HtmlWindow;

        public HtmlViewEngine(IWebBrowserWindowProvider htmlWindowProvider, IJavascriptFrameworkManager frameworkManager, IWebSessionLogger logger)
        {
            _HtmlWindowProvider = htmlWindowProvider;
            _FrameworkManager = frameworkManager;
            Logger = logger;
        }

        public HtmlViewContext GetMainContext()
        {
            return new HtmlViewContext(HtmlWindow, _HtmlWindowProvider.UiDispatcher, _FrameworkManager, Logger);
        }

        internal BidirectionalMapper GetMapper(object viewModel, JavascriptBindingMode mode, IJavascriptObjectBuilderStrategyFactory strategyFactory)
        {
            return new BidirectionalMapper(viewModel, this, mode, Logger, strategyFactory);
        }

        public T Evaluate<T>(Func<T> compute)
        {
            return MainView.Evaluate(compute);
        }
    }
}
