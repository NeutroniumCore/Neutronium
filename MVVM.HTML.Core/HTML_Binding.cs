using System;
using System.Threading.Tasks;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Binding;

namespace MVVM.HTML.Core
{
    public class HTML_Binding : IDisposable, IHTMLBinding
    {
        private BidirectionalMapper _BirectionalMapper;
        private IWebView _CefV8Context;

        private HTML_Binding(IWebView iContext, BidirectionalMapper iConvertToJSO)
        {
            _CefV8Context = iContext;
            _BirectionalMapper = iConvertToJSO;
        }

        public IJavascriptObject JSRootObject
        {
            get { return _BirectionalMapper.JSValueRoot.GetJSSessionValue(); }
        }

        public IWebView Context { get { return _CefV8Context; } }

        public object Root
        {
            get { return _BirectionalMapper.JSValueRoot.CValue; }
        }

        public IJSCSGlue JSBrideRootObject
        {
            get { return _BirectionalMapper.JSValueRoot; }
        }

        public override string ToString()
        {
            return _BirectionalMapper.JSValueRoot.ToString();
        }

        public void Dispose()
        {
            _BirectionalMapper.Dispose();
        }

        internal static async Task<IHTMLBinding> Bind(HTMLViewEngine viewEngine, object iViewModel, JavascriptBindingMode iMode, object additional = null)
        {
            var windowprovider = viewEngine.HTMLWindowProvider;
            var context = windowprovider.HTMLWindow.MainFrame;

            var htmlContext = new HTMLViewContext(context, windowprovider.UIDispatcher, viewEngine.SessionInjectorFactory);
            var mapper = await context.EvaluateAsync(() => new BidirectionalMapper(iViewModel, htmlContext, iMode, additional));
            await mapper.Init();
            return new HTML_Binding(context, mapper);
        }
    }
}
