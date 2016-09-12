using Neutronium.Core.JavascriptEngine.JavascriptObject;

namespace Neutronium.Core.JavascriptFramework
{
    /// <summary>
    /// javascript observer for changes occuring on ViewModel
    /// </summary>
    public interface IJavascriptChangesObserver
    {
        /// <summary>
        /// Method to be called when an object property value has changed
        /// </summary>
        /// <param name="objectChanged">
        /// Object that changes
        /// </param>
        /// <param name="propertyName">
        /// Name of the property that changed
        /// </param>
        /// <param name="newValue">
        /// New value for the property
        /// </param>
        void OnJavaScriptObjectChanges(IJavascriptObject objectChanged, string propertyName, IJavascriptObject newValue);

        /// <summary>
        /// Method to be called when a collection has changed
        /// </summary>
        /// <param name="changes">
        /// Changes that happened to the collection
        /// </param>
        void OnJavaScriptCollectionChanges(JavascriptCollectionChanges changes);
    }
}
