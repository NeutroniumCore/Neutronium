using System.Threading.Tasks;

namespace Neutronium.MVVMComponents
{
    /// <summary>
    /// Command that triggers a result
    /// </summary>
    public interface IResultCommand<TResult>
    {
        /// <summary>
        /// return asynchronously a result given a specific argument
        /// </summary>
        /// <returns></returns>
        Task<TResult> Execute();
    }
}
