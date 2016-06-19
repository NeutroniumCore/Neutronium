using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.JavascriptEngine.JavascriptObject
{
    /// <summary>
    /// Abstraction of a javascript object
    /// </summary>
    public interface IJavascriptObject : IDisposable
    {
        bool HasRelevantId();

        uint GetID();

        /// <summary>
        /// True if the value type is Undefined.
        /// </summary>
        bool IsUndefined { get;  }

        /// <summary>
        /// True if the value type is Null.
        /// </summary>
        bool IsNull { get; }

        /// <summary>
        /// True if the value type is an object.
        /// </summary>
        bool IsObject { get; }

        /// <summary>
        /// True if the value type is an array.
        /// </summary>
        bool IsArray { get;  }

        /// <summary>
        /// True if the value type is string.
        /// </summary>
        bool IsString { get; }

        /// <summary>
        /// True if the value type is an number.
        /// </summary>
        bool IsNumber { get; }

        /// <summary>
        /// True if the value type is an boolean.
        /// </summary>
        bool IsBool { get; }

        /// <summary>
        /// Only available on arrays.
        /// Returns the number of elements in the array.
        /// </summary>
        int GetArrayLength();

        /// <summary>
        /// Returns true if the object has a value with the specified identifier.
        /// </summary>
        bool HasValue(string attributename);

        /// <summary>
        /// Execute synchronously the function in the coresponding IWebView context.
        /// </summary>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="iFunctionName">
        /// function name
        /// </param>
        /// <param name="iparam">
        /// function parameters
        /// </param>
        /// <returns>
        /// the result of the execution
        ///</returns>
        IJavascriptObject Invoke(string iFunctionName, IWebView context, params IJavascriptObject[] iparam);

        /// <summary>
        /// Execute asynchronously the function in the coresponding IWebView context.
        /// </summary>
        /// <param name="iFunctionName">
        /// function name
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="iparam">
        /// function parameters
        /// </param>
        /// <returns>
        /// task of the result of the execution
        ///</returns>
        Task<IJavascriptObject> InvokeAsync(string iFunctionName, IWebView context, params IJavascriptObject[] iparam);

        /// <summary>
        /// Bind a function to a jaavscript object
        /// </summary>
        /// <param name="iFunctionName">
        /// function name
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="action">
        /// action to run when the function is called. Action agrument: 
        /// function name, this object, function parameters
        /// </param>
        void Bind(string iFunctionName, IWebView context, Action<string, IJavascriptObject, IJavascriptObject[]> action);

        /// <summary>
        /// Associates a value with the specified identifier.
        /// </summary>
        void SetValue(string AttributeName, IJavascriptObject element, CreationOption ioption = CreationOption.None);

        /// <summary>
        /// Return a string value.  The underlying data will be converted to if
        /// necessary.
        /// </summary>
        string GetStringValue();

        /// <summary>
        /// Return a double value.  The underlying data will be converted to if
        /// necessary.
        /// </summary>
        double GetDoubleValue();

        /// <summary>
        /// Return a boolean value.  The underlying data will be converted to if
        /// necessary.
        /// </summary>
        bool GetBoolValue();

        /// <summary>
        /// Return an int value.  The underlying data will be converted to if
        /// necessary.
        /// </summary>
        int GetIntValue();

        /// <summary>
        /// Execute the function with no argument.
        /// </summary>
        /// <param name="context">
        /// function context
        /// </param>
        IJavascriptObject ExecuteFunction(IWebView context);

        /// <summary>
        /// Returns the value with the specified identifier on success. Returns NULL
        /// if this method is called incorrectly or an exception is thrown.
        /// </summary>
        IJavascriptObject GetValue(string ivalue);

        /// <summary>
        /// Get the values of the attributes if the javascript object is an object
        /// </summary>
        IEnumerable<string> GetAttributeKeys();

        /// <summary>
        /// Only available on arrays.
        /// Returns the value with the specified index on success. Returns NULL
        /// if this method is called incorrectly or an exception is thrown.
        /// </summary>
        IJavascriptObject GetValue(int index);

        /// <summary>
        /// Only available on arrays.
        /// Returns the elements in the array.
        /// </summary>
        IJavascriptObject[] GetArrayElements();
    }
}
