using System.Threading.Tasks;

namespace Neutronium.Core.Binding
{
    internal class BindingBuilder : IBindingBuilder
    {
        private readonly HtmlBinding _Binding;
        private readonly BidirectionalMapper _Mapper;

        internal BindingBuilder(BidirectionalMapper mapper)
        {
            _Binding = new HtmlBinding(mapper);
            _Mapper = mapper;
        }

        async Task<IHtmlBinding> IBindingBuilder.CreateBinding(bool debugMode)
        {
            await _Mapper.UpdateJavascriptObjects(debugMode);
            return _Binding;
        }
    }
}
