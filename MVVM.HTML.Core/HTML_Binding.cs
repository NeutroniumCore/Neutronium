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
        private static IHTMLBinding _IHTMLBinding;

        public IJavascriptSessionInjector JavascriptUIFramework => _Context.JavascriptSessionInjector;
        public IJavascriptObject JSRootObject => _BirectionalMapper.JSValueRoot.GetJSSessionValue();
        public JavascriptBindingMode Mode => _BirectionalMapper.Mode;
        public IWebView Context => _Context.WebView;
        public object Root => _BirectionalMapper.JSValueRoot.CValue;
        public IJSCSGlue JSBrideRootObject => _BirectionalMapper.JSValueRoot;
        private readonly IWebSessionLogger _Logger;
        private static int _Count = 0;
        private int _Current = _Count++;

        private HTML_Binding(BidirectionalMapper iConvertToJSO, IWebSessionLogger logger)
        {
            _Context = iConvertToJSO.Context;
            _BirectionalMapper = iConvertToJSO;
            _Logger = logger;
            _Logger.Debug(() => $"HTML_Binding {_Current} created");
        }

        ~HTML_Binding()
        {
            _Logger.Debug(() => $"HTML_Binding {_Current} finalized");
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
            var mapper = viewEngine.GetMapper(iViewModel, iMode, additional);
            var res =
                _IHTMLBinding =
                new HTML_Binding(mapper, viewEngine.Logger);
            await viewEngine.Evaluate( () => mapper.Init());
            return res;
         }
    }
}
