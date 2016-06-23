using System.Threading.Tasks;

namespace MVVM.HTML.Core.JavascriptUIFramework
{
    public interface  IJavascriptObjectInternalMapper : IJavascriptObjectMapper
    {
        /// <summary>
        /// Task that complete when mapping is done (after EndMapping is called).
        /// </summary>
        Task UpdateTask { get; }
    }
}
