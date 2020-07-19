using System.Threading.Tasks;

namespace Neutronium.Core.Binding
{
    internal class BindingBuilder : IBindingBuilder
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
