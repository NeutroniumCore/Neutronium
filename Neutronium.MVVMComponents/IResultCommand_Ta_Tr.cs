using System.Threading.Tasks;

namespace Neutronium.MVVMComponents
{
    /// <summary>
    /// Command that triggers a result
    /// </summary>
    public interface IResultCommand<in TArg, TResult>
    {
        /// <summary>
        /// return asynchronously a result given a specific argument
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        Task<TResult> Execute(TArg argument);
    }
}
