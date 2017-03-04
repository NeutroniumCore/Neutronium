using System.Threading.Tasks;

namespace Neutronium.MVVMComponents
{
    public interface IResultCommand
    {
        Task<object> Execute(object argument);
    }
}
