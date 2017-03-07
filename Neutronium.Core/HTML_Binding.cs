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

        private HTML_Binding(BidirectionalMapper convertToJSO, IWebSessionLogger logger)
        {
            _Context = convertToJSO.Context;
            _BirectionalMapper = convertToJSO;
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

        internal static async Task<IHTMLBinding> Bind(HTMLViewEngine viewEngine, object viewModel, JavascriptBindingMode mode, object additional = null)
        {
            var builder = await GetBindingBuilder(viewEngine, viewModel, mode, additional);
            return await builder.CreateBinding(false);
        }

        internal static async Task<IBindingBuilder> GetBindingBuilder(HTMLViewEngine viewEngine, object viewModel, JavascriptBindingMode mode, object additional = null) 
        {
            var mapper = viewEngine.GetMapper(viewModel, mode);
            var bindingBuilder = new BindingBuilder(mapper, viewEngine.Logger, additional);
            await bindingBuilder.Init();
            return bindingBuilder;
        }

        private class BindingBuilder : IBindingBuilder
        {
            private readonly HTML_Binding _Binding;
            private readonly BidirectionalMapper _Mapper;
            private readonly object _AdditionalVm;
            public BindingBuilder(BidirectionalMapper mapper, IWebSessionLogger logger, object additionalVm) 
            {
                _Binding = new HTML_Binding(mapper, logger);
                _Mapper = mapper;
                _AdditionalVm = additionalVm;
            }

            public Task Init() 
            {
                return _Mapper.IntrospectVm(_AdditionalVm);
            }

            async Task<IHTMLBinding> IBindingBuilder.CreateBinding(bool debugMode) 
            {
                await _Mapper.UpdateJavascriptObjects(debugMode);
                return _Binding;
            }
        }
    }
}
