using System.Threading.Tasks;

namespace MVVM.Component
{
    public interface IResultCommand
    {
        Task<object> Execute(object iargument);
    }
}
