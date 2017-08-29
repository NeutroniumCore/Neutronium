using System.Threading.Tasks;

namespace Neutronium.Core 
{
    internal interface IBindingBuilder 
    {
        Task<IHtmlBinding> CreateBinding(bool debugMode);
    }
}