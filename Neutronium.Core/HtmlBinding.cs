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
        private readonly BidirectionalMapper _BirectionalMapper;
        private readonly HtmlViewContext _Context;
        private readonly IWebSessionLogger _Logger;
        private readonly int _Current = _Count++;

        public IJavascriptSessionInjector JavascriptUiFramework => _Context.JavascriptSessionInjector;
        public IJavascriptObject JsRootObject => _BirectionalMapper.JsValueRoot.GetJsSessionValue();
        public JavascriptBindingMode Mode => _BirectionalMapper.Mode;
        public IWebView Context => _Context.WebView;
        public object Root => _BirectionalMapper.JsValueRoot.CValue;
        public IJsCsGlue JsBrideRootObject => _BirectionalMapper.JsValueRoot;

        private HtmlBinding(BidirectionalMapper convertToJso, IWebSessionLogger logger)
        {
            _Context = convertToJso.Context;
            _BirectionalMapper = convertToJso;
            _Logger = logger;           
            _Bindings.Add(this);
            _Logger.Debug(() => $"HTML_Binding {_Current} created");
        }

        public override string ToString()
        {
            return _BirectionalMapper.JsValueRoot.ToString();
        }

        public void Dispose()
        {            
            _BirectionalMapper.Dispose();
            _Bindings.Remove(this);
            _Logger.Debug(() => $"HTML_Binding {_Current} disposed");
        }

        internal static async Task<IHtmlBinding> Bind(HtmlViewEngine viewEngine, object viewModel, JavascriptBindingMode mode)
        {
            var builder = GetBindingBuilder(viewEngine, viewModel, mode);
            return await builder.CreateBinding(false);
        }

        internal static async Task<IHtmlBinding> Bind(HtmlViewEngine viewEngine, object viewModel, JavascriptBindingMode mode, IJavascriptObjectBuilderStrategyFactory strategyFactory)
        {
            var builder = GetBindingBuilder(viewEngine, viewModel, mode, strategyFactory);
            return await builder.CreateBinding(false);
        }

        internal static IBindingBuilder GetBindingBuilder(HtmlViewEngine viewEngine, object viewModel, JavascriptBindingMode mode, IJavascriptObjectBuilderStrategyFactory strategyFactory= null) 
        {
            var mapper = viewEngine.GetMapper(viewModel, mode, strategyFactory);
            return new BindingBuilder(mapper, viewEngine.Logger);
        }

        private class BindingBuilder : IBindingBuilder
        {
            private readonly HtmlBinding _Binding;
            private readonly BidirectionalMapper _Mapper;

            public BindingBuilder(BidirectionalMapper mapper, IWebSessionLogger logger) 
            {
                _Binding = new HtmlBinding(mapper, logger);
                _Mapper = mapper;
            }

            async Task<IHtmlBinding> IBindingBuilder.CreateBinding(bool debugMode) 
            {
                await _Mapper.UpdateJavascriptObjects(debugMode);
                return _Binding;
            }
        }
    }
}
