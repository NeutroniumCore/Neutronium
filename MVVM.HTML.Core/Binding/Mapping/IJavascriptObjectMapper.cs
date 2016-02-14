using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    /// <summary>
    /// Mapper used to create corresponde beetween simple javascript object
    /// and corresponding observable javascript object
    /// </summary>
    public interface IJavascriptObjectMapper
    {
        /// <summary>
        /// Map the root object
        /// </summary>
        void MapFirst(IJavascriptObject rootObject);

        /// <summary>
        /// Map a child object relative to its father
        /// </summary>
        /// <param name="father">
        /// father observable object
        /// </param>
        /// <param name="attribute">
        /// Attribute name
        /// </param>
        /// </param>
        /// <param name="child">
        /// child observable object to be mapped
        /// </param>
        void Map(IJavascriptObject father, string att, IJavascriptObject child);

        /// <summary>
        /// Map a child object in a collection relative to its father and index
        /// </summary>
        /// <param name="father">
        /// father observable object
        /// </param>
        /// <param name="attribute">
        /// Attribute name of the collection
        /// </param>
        /// </param>
        /// <param name="index">
        /// index of the child observable object in the collection
        /// </param>
        /// </param>
        /// <param name="child">
        /// child observable object to be mapped
        /// </param>
        void MapCollection(IJavascriptObject father, string attribute, int index, IJavascriptObject child);

        /// <summary>
        /// Method to be called when maping ends
        /// </summary>
        /// <param name="rootObject">
        /// root observable object
        /// </param>
        void EndMapping(IJavascriptObject rootObject);

        /// <summary>
        /// Task that complete when mapping is done (after EndMapping is called).
        /// </summary>
        Task UpdateTask { get; }
    }
}
