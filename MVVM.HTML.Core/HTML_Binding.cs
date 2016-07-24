using System.Threading.Tasks;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core
{
    public class HTML_Binding : IHTMLBinding
    {
        private readonly BidirectionalMapper _BirectionalMapper;
        private readonly HTMLViewContext _Context;

        public IJavascriptSessionInjector JavascriptUIFramework => _Context.JavascriptSessionInjector;
        public IJavascriptObject JSRootObject => _BirectionalMapper.JSValueRoot.GetJSSessionValue();
        public JavascriptBindingMode Mode => _BirectionalMapper.Mode;
        public IWebView Context => _Context.WebView;
        public object Root => _BirectionalMapper.JSValueRoot.CValue;
        public IJSCSGlue JSBrideRootObject => _BirectionalMapper.JSValueRoot;

        private HTML_Binding(BidirectionalMapper iConvertToJSO)
        {
            _Context = iConvertToJSO.Context;
            _BirectionalMapper = iConvertToJSO;
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
            var mapper = await viewEngine.GetMapper(iViewModel, iMode, additional);
            return new HTML_Binding(mapper);
        }
    }
}
