using System.Threading.Tasks;

namespace Neutronium.MVVMComponents
{
    /// <summary>
    /// Command that triggers a result
    /// </summary>
    public interface IResultCommand
    {
        /// <summary>
        /// return asynchroneously a result given a specific argument
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        Task<object> Execute(object argument);
    }
}
