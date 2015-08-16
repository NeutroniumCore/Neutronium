using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject
{
    /// <summary>
    /// Factory to create IJavascriptObject
    /// </summary>
    public interface IJavascriptObjectFactory 
    {
        /// <summary>
        /// Create IJavascriptObject from basic CLR types
        /// </summary>
        /// <param name="ifrom">
        /// object to be converted
        /// </param>
        /// <param name="res">
        /// IJavascriptObject converted from <paramref name="ifrom" />
        /// </param>
        /// <returns>
        /// true if the IJavascriptObject has been created
        ///</returns>
        bool SolveBasic(object ifrom, out IJavascriptObject res);

        /// <summary>
        /// Indicates if a type is a basic CLR type which
        /// can be converted with SolveBasic
        /// </summary>
        /// <param name="itype">
        /// Type to check
        /// </param>
        /// <returns>
        /// true if type can be handled by SolveBasic
        ///</returns>
        bool IsTypeBasic(Type itype);

        /// <summary>
        /// Create IJavascriptObject representing null value
        /// </summary>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateNull();

        /// <summary>
        /// Create IJavascriptObject object
        /// </summary>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateObject();

        /// <summary>
        /// Create IJavascriptObject from an int
        /// </summary>
        /// <param name="value">
        /// int to be converted
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateInt(int value);

        /// <summary>
        /// Create IJavascriptObject from an double
        /// </summary>
        /// <param name="value">
        /// double to be converted
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateDouble(double value);


        /// <summary>
        /// Create IJavascriptObject from an string
        /// </summary>
        /// <param name="value">
        /// string to be converted
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateString(string value);


        /// <summary>
        /// Create IJavascriptObject from a boolean
        /// </summary>
        /// <param name="value">
        /// boolean to be converted
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateBool(bool value);

        /// <summary>
        /// Create IJavascriptObject list
        /// </summary>
        /// <param name="iCount">
        /// the initial size of the collection
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateArray(int iCount);


        /// <summary>
        /// Create IJavascriptObject from enum
        /// </summary>
        /// <param name="value">
        /// enum to convert
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateEnum(Enum value);
    }
}
