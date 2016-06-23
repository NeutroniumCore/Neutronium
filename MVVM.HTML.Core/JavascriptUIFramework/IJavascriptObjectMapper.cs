using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.JavascriptUIFramework
{
    /// <summary>
    /// Mapper used to create corresponde beetween simple javascript object
    /// and corresponding observable javascript object
    /// </summary>
    public interface IJavascriptObjectMapper 
    {
        /// <summary>
        /// Should be called when injector does not need to create new VM javascrit object
        /// and will reuse the objects provided by HTML core.
        /// Important: This method should be called in the WebView Context (RunAsync...).
        /// Important: this method should be called once. Calling any other methods after 
        /// calling AutoMap will throw an exception. Calling AutoMap after calling another 
        /// mapper method will also raise an exception.
        /// </summary>
        void AutoMap();

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
        /// <param name="child">
        /// child observable object to be mapped
        /// </param>
        void Map(IJavascriptObject father, string attribute, IJavascriptObject child);

        /// <summary>
        /// Map a child object in a collection relative to its father and index
        /// </summary>
        /// <param name="father">
        /// father observable object
        /// </param>
        /// <param name="attribute">
        /// Attribute name of the collection
        /// </param>
        /// <param name="index">
        /// index of the child observable object in the collection
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
    }
}
