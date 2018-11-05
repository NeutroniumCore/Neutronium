using System;
using System.Collections.Generic;
using Neutronium.Core.Infra.Reflection;

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
        /// objects to be converted
        /// </param>
        /// <returns>
        /// IJavascriptObjects created
        ///</returns>
        IEnumerable<IJavascriptObject> CreateFromExcecutionCode(IEnumerable<string> from);

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
        /// <param name="objectObservability">
        /// the corresponding object ObjectObservability. 
        /// Readonly object have the property NeutroniumConstants.ReadOnlyFlag ("__readonly__") set to true
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateObject(ObjectObservability objectObservability);

        /// <summary>
        /// Create IJavascriptObject objects in bulk
        /// </summary> 
        /// <param name="option">
        /// number of read only and read-write objects to create. 
        /// Readonly object have the property NeutroniumConstants.ReadOnlyFlag ("__readonly__") set to true
        /// </param>
        /// <returns>
        /// corresponding collection of IJavascriptObject
        ///</returns>
        IEnumerable<IJavascriptObject> CreateObjects(ObjectsCreationOption option);

        /// <summary>
        /// Create IJavascriptObject object from constructor
        /// </summary> 
        /// <param name="number">
        /// number of objects to create
        /// </param>
        /// <param name="constructor">
        /// constructor object
        /// </param>
        /// <param name="parameters">
        /// constructor parameters
        /// </param>
        /// <returns>
        /// corresponding collection of IJavascriptObject
        ///</returns>
        IEnumerable<IJavascriptObject> CreateObjectsFromConstructor(int number, IJavascriptObject constructor, params IJavascriptObject[] parameters);


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
        /// Create IJavascriptObject from an uint
        /// </summary>
        /// <param name="value">
        /// int to be converted
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateUint(uint value);

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
        /// Create IJavascriptObject from a DateTime
        /// </summary>
        /// <param name="value">
        /// boolean to be converted
        /// </param>
        /// <returns>
        /// corresponding IJavascriptObject
        ///</returns>
        IJavascriptObject CreateDateTime(DateTime value);

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
