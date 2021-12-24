using System.Threading.Tasks;

namespace Neutronium.Core 
{
    public interface IBindingBuilder 
    {
        Task<IHtmlBinding> CreateBinding(bool debugMode);
    }
}