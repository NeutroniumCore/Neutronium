using System.Threading.Tasks;

namespace Neutronium.Core.JavascriptUIFramework
{
    public interface  IJavascriptObjectInternalMapper : IJavascriptObjectMapper
    {
        /// <summary>
        /// Task that complete when mapping is done (after EndMapping is called).
        /// </summary>
        Task UpdateTask { get; }
    }
}
