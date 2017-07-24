using System;
using System.Collections.Generic;

namespace Neutronium.Core.WebBrowserEngine.JavascriptObject
{
    /// <summary>
    /// Factory to create IJavascriptObject
    /// </summary>
    public interface IJavascriptObjectFactory 
    {
        /// <summary>
        /// Create IJavascriptObject from basic CLR types
        /// </summary>
        /// <param name="from">
        /// object to be converted
        /// </param>
        /// <param name="res">
        /// IJavascriptObject converted from <paramref name="from" />
        /// </param>
        /// <returns>
        /// true if the IJavascriptObject has been created
        ///</returns>
        bool CreateBasic(object from, out IJavascriptObject res);

        /// <summary>
        /// Create IJavascriptObject from basic CLR types
        /// </summary>
        /// <param name="from">
        /// objects to be converted
        /// </param>
        /// <returns>
        /// IJavascriptObjects created
        ///</returns>
        IEnumerable<IJavascriptObject> CreateBasics(IReadOnlyList<object> from);

        /// <summary>
        /// Indicates if a type is a basic CLR type which
        /// can be converted with SolveBasic
        /// </summary>
        /// <param name="type">
        /// Type to check
        /// </param>
        /// <returns>
        /// true if type can be handled by SolveBasic
        ///</returns>
        bool IsTypeBasic(Type type);

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
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateObject();

        /// <summary>
        /// Create IJavascriptObject object
        /// </summary> 
        /// <param name="readOnly">
        /// true if local readOnly. 
        /// Readonly object have the property NeutroniumConstants.ReadOnlyFlag ("__readonly__") set to true
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateObject(bool readOnly);

        /// <summary>
        /// Create IJavascriptObject objects in bulk
        /// </summary> 
        /// <param name="readOnlyNumber">
        /// number of read only objects to create. 
        /// Readonly object have the property NeutroniumConstants.ReadOnlyFlag ("__readonly__") set to true
        /// </param>
        /// <param name="readWrite">
        /// number of read write objects to create. 
        /// </param>
        /// <returns>
        /// corresponding collection of IJavascriptObject
        ///</returns>
        IEnumerable<IJavascriptObject> CreateObjects(int readWrite, int readOnlyNumber);

        /// <summary>
        /// Create IJavascriptObject object from javascript code
        /// </summary>   
        /// <param name="creationCode">
        /// the javascript code to create the object
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateObject(string creationCode);

        /// <summary>
        /// Create IJavascriptObject corresponding to undefined
        /// </summary>   
        IJavascriptObject CreateUndefined();

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
        /// <param name="collection">
        /// the elements of the collection
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateArray(IEnumerable<IJavascriptObject> collection);


        /// <summary>
        /// Create IJavascriptObject arrays in bulk
        /// </summary>   
        /// <param name="number">
        /// number of array to create. 
        /// </param>
        /// <returns>
        /// corresponding collection of IJavascriptObject
        ///</returns>
        IEnumerable<IJavascriptObject> CreateArrays(int number);

        /// <summary>
        /// Create IJavascriptObject list
        /// </summary>
        /// <param name="size">
        /// the size of the collection
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateArray(int size);
    }
}
