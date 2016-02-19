using MVVM.HTML.Core.HTMLBinding;
using System;
using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Binding.Mapping
{
    /// <summary>
    /// Abstraction of the javascript framework responsible for databind
    /// and change tracking
    /// </summary>
    public interface IJavascriptSessionInjector : IDisposable
    {
        /// <summary>
        /// Maps a simple javascript object to an obseravble javscript object
        /// </summary>
        /// <param name="rawObject">
        /// Simple javascript to be mapped
        /// </param>
        /// <param name="mapper">
        /// Mapper to be used to map the original object and the observable one
        /// </param>
        /// <returns>
        /// the corresponding observable javascript object
        ///</returns>
        IJavascriptObject Inject(IJavascriptObject rawObject, IJavascriptObjectMapper mapper);


        /// <summary>
        /// Register main view model in javascript windows
        /// </summary>
        /// <param name="rawObject">
        /// Main ViewModel: javascript Observable object 
        /// </param>
        /// <returns>
        /// task that completes when all binding are done
        ///</returns>
        Task RegisterMainViewModel(IJavascriptObject jsObject);

        /// <summary>
        /// Update javascript viewmodel without raising listeners events
        /// </summary>
        /// <param name="father">
        /// view model to be updated
        /// </param>
        /// <param name="propertyName">
        /// Name of the property to be updated
        /// </param>
        /// <param name="value">
        /// new value of the property
        /// </param>
        void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value);

        void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject glue);

        void SpliceCollection(IJavascriptObject array,int index, int number);

        void ClearAllCollection(IJavascriptObject array);

        void MoveCollectionItem(IJavascriptObject array, IJavascriptObject item, int oldIndex, int newIndex);
    }
}
