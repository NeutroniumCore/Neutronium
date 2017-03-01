using System.Threading.Tasks;

namespace Neutronium.Core 
{
    internal interface IBindingBuilder 
    {
        Task<IHTMLBinding> CreateBinding();
    }
}