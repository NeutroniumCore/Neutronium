using System;

namespace MVVM.HTML.Core.V8JavascriptObject
{
    /// <summary>
    /// Converter from IJavascriptObject to basic CLR Type
    /// </summary>
    public interface IJavascriptObjectConverter
    {
        /// <summary>
        /// Convert a IJavascriptObject to basic CLR Type
        /// </summary>
        /// <param name="value">
        /// IJavascriptObject to convert
        /// </param>
        /// <param name="res">
        /// converted object
        /// </param>
        /// <param name="iTargetType">
        /// Target type for the result if any
        /// </param>
        /// <returns>
        /// true if the operation is successfull
        ///</returns>
        bool GetSimpleValue(IJavascriptObject value, out object res, Type iTargetType = null);
    }
}
