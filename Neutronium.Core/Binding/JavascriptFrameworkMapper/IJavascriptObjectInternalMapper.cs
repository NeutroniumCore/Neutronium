using System.Threading.Tasks;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core.Binding.JavascriptFrameworkMapper
{
    public interface  IJavascriptObjectInternalMapper : IJavascriptObjectMapper
    {
        /// <summary>
        /// Task that complete when mapping is done (after EndMapping is called).
        /// </summary>
        Task UpdateTask { get; }
    }
}
