using System.Collections.Generic;
using System.Threading.Tasks;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core
{
    public class HTML_Binding : IHTMLBinding
    {
        private static int _Count;
        private static readonly HashSet<IHTMLBinding> _Bindings = new HashSet<IHTMLBinding>();
        private readonly BidirectionalMapper _BirectionalMapper;
        private readonly HTMLViewContext _Context;
        private readonly IWebSessionLogger _Logger;
        private readonly int _Current = _Count++;

        public IJavascriptSessionInjector JavascriptUIFramework => _Context.JavascriptSessionInjector;
        public IJavascriptObject JSRootObject => _BirectionalMapper.JSValueRoot.GetJSSessionValue();
        public JavascriptBindingMode Mode => _BirectionalMapper.Mode;
        public IWebView Context => _Context.WebView;
        public object Root => _BirectionalMapper.JSValueRoot.CValue;
        public IJSCSGlue JSBrideRootObject => _BirectionalMapper.JSValueRoot;


        private HTML_Binding(BidirectionalMapper iConvertToJSO, IWebSessionLogger logger)
        {
            _Context = iConvertToJSO.Context;
            _BirectionalMapper = iConvertToJSO;
            _Logger = logger;
            _Logger.Debug(() => $"HTML_Binding {_Current} created");
            _Bindings.Add(this);
        }

        public override string ToString()
        {
            return _BirectionalMapper.JSValueRoot.ToString();
        }

        public void Dispose()
        {
            _Logger.Debug(() => $"HTML_Binding {_Current} diposed");
            _BirectionalMapper.Dispose();
            _Bindings.Remove(this);
        }

        internal static async Task<IHTMLBinding> Bind(HTMLViewEngine viewEngine, object iViewModel, JavascriptBindingMode iMode, object additional = null)
        {
            var mapper = viewEngine.GetMapper(iViewModel, iMode, additional);
            var res = new HTML_Binding(mapper, viewEngine.Logger);
            await viewEngine.Evaluate( () => mapper.Init());
            return res;
         }
    }
}
