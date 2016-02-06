using System.Threading.Tasks;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Binding.Mapping;

namespace MVVM.HTML.Core
{
    public class HTML_Binding : IHTMLBinding
    {
        private readonly BidirectionalMapper _BirectionalMapper;
        private readonly HTMLViewContext _Context;

        private HTML_Binding(HTMLViewContext iContext, BidirectionalMapper iConvertToJSO)
        {
            _Context = iContext;
            _BirectionalMapper = iConvertToJSO;
        }

        public IJavascriptSessionInjector JavascriptUIFramework
        {
            get { return _Context.JavascriptSessionInjector; }
        }

        public IJavascriptObject JSRootObject
        {
            get { return _BirectionalMapper.JSValueRoot.GetJSSessionValue(); }
        }

        public IWebView Context { get { return _Context.WebView; } }

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
            var htmlContext = viewEngine.GetMainContext();
            var mapper = await htmlContext.GetMapper(iViewModel, iMode, additional);
            return new HTML_Binding(htmlContext, mapper);
        }
    }
}
