using System.Collections.Generic;
using System.Threading.Tasks;
using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;

namespace Neutronium.Core
{
    public class HtmlBinding : IHtmlBinding
    {
        private static int _Count;
        private static readonly HashSet<IHtmlBinding> _Bindings = new HashSet<IHtmlBinding>();
        private readonly BidirectionalMapper _BidirectionalMapper;
        private readonly HtmlViewContext _Context;
        private readonly IWebSessionLogger _Logger;
        private readonly int _Current = _Count++;

        public IJavascriptSessionInjector JavascriptUiFramework => _Context.JavascriptSessionInjector;
        public IJavascriptObject JsRootObject => _BidirectionalMapper.JsValueRoot.GetJsSessionValue();
        public JavascriptBindingMode Mode => _BidirectionalMapper.Mode;
        public IWebView Context => _Context.WebView;
        public object Root => _BidirectionalMapper.JsValueRoot.CValue;
        public IJsCsGlue JsBrideRootObject => _BidirectionalMapper.JsValueRoot;

        internal HtmlBinding(BidirectionalMapper bidirectionalMapper)
        {
            _Context = bidirectionalMapper.Context;
            _BidirectionalMapper = bidirectionalMapper;
            _Logger = _Context.Logger;           
            _Bindings.Add(this);
            _Logger.Debug(() => $"HTML_Binding {_Current} created");
        }

        public override string ToString()
        {
            return _BidirectionalMapper.JsValueRoot.ToString();
        }

        public void Dispose()
        {            
            _BidirectionalMapper.Dispose();
            _Bindings.Remove(this);
            _Logger.Debug(() => $"HTML_Binding {_Current} disposed");
        }

        public static async Task<IHtmlBinding> Bind(HtmlViewEngine viewEngine, object viewModel, JavascriptBindingMode mode)
        {
            var builder = GetBindingBuilder(viewEngine, viewModel, mode);
            return await builder.CreateBinding(false);
        }

        public static async Task<IHtmlBinding> Bind(HtmlViewEngine viewEngine, object viewModel, JavascriptBindingMode mode, IJavascriptObjectBuilderStrategyFactory strategyFactory)
        {
            var builder = GetBindingBuilder(viewEngine, viewModel, mode, strategyFactory);
            return await builder.CreateBinding(false);
        }

        public static IBindingBuilder GetBindingBuilder(HtmlViewEngine viewEngine, object viewModel, JavascriptBindingMode mode, IJavascriptObjectBuilderStrategyFactory strategyFactory= null) 
        {
            var mapper = viewEngine.GetMapper(viewModel, mode, strategyFactory);
            return new BindingBuilder(mapper);
        }
    }
}
