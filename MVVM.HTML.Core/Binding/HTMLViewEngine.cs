using MVVM.HTML.Core.JavascriptEngine;
using MVVM.HTML.Core.V8JavascriptObject;

namespace MVVM.HTML.Core.Binding
{
    public class HTMLViewEngine
    {
        internal HTMLViewEngine(IHTMLWindowProvider hTMLWindowProvider, IJavascriptSessionInjectorFactory sessionInjectorFactory)
        {
            HTMLWindowProvider = hTMLWindowProvider;
            SessionInjectorFactory = sessionInjectorFactory;
        }

        private IHTMLWindowProvider HTMLWindowProvider { get;  set; }

        private IJavascriptSessionInjectorFactory SessionInjectorFactory { get; set; }

        public HTMLViewContext GetContext()
        {
            return new HTMLViewContext(MainView, HTMLWindowProvider.UIDispatcher, SessionInjectorFactory);
        }

        public IWebView MainView
        {
            get { return HTMLWindowProvider.HTMLWindow.MainFrame; }
        }
    }
}
