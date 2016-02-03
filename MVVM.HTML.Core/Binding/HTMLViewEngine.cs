using MVVM.HTML.Core.JavascriptEngine;

namespace MVVM.HTML.Core.Binding
{
    public class HTMLViewEngine
    {
        internal HTMLViewEngine(IHTMLWindowProvider hTMLWindowProvider, IJavascriptSessionInjectorFactory sessionInjectorFactory)
        {
            HTMLWindowProvider = hTMLWindowProvider;
            SessionInjectorFactory = sessionInjectorFactory;
        }

        public IHTMLWindowProvider HTMLWindowProvider { get; private set; }

        public IJavascriptSessionInjectorFactory SessionInjectorFactory { get; private set; }
    }
}
