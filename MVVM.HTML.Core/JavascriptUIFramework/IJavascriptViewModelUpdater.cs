using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.JavascriptUIFramework
{
    /// <summary>
    /// Interface responsible for updating javascript ViewModel to reflect
    /// C# ViewModel updates
    /// </summary>
    public interface IJavascriptViewModelUpdater
    {
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

        void SpliceCollection(IJavascriptObject array, int index, int number);

        void ClearAllCollection(IJavascriptObject array);

        void MoveCollectionItem(IJavascriptObject array, IJavascriptObject item, int oldIndex, int newIndex);
    }
}
