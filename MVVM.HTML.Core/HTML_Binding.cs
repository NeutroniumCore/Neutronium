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

        private HTML_Binding(BidirectionalMapper iConvertToJSO)
        {
            _Context = iConvertToJSO.Context;
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

        public JavascriptBindingMode Mode { get { return _BirectionalMapper.Mode; } }

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
            var mapper = await viewEngine.GetMapper(iViewModel, iMode, additional);
            return new HTML_Binding(mapper);
        }
    }
}
