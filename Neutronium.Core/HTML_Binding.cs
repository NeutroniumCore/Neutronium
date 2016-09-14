using System.Collections.Generic;
using System.Threading.Tasks;
using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core
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
            _Bindings.Add(this);
            _Logger.Debug(() => $"HTML_Binding {_Current} created");
        }

        public override string ToString()
        {
            return _BirectionalMapper.JSValueRoot.ToString();
        }

        public void Dispose()
        {            
            _BirectionalMapper.Dispose();
            _Bindings.Remove(this);
            _Logger.Debug(() => $"HTML_Binding {_Current} disposed");
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
