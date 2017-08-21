using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neutronium.Core.WebBrowserEngine.JavascriptObject
{
    /// <summary>
    /// Abstraction of a javascript object
    /// </summary>
    public interface IJavascriptObject : IDisposable
    {
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
        /// <param name="functionName">
        /// function name
        /// </param>
        /// <param name="param">
        /// function parameters
        /// </param>
        /// <returns>
        /// the result of the execution
        ///</returns>
        IJavascriptObject Invoke(string functionName, IWebView context, params IJavascriptObject[] param);

        /// <summary>
        /// Execute synchronously the function in the coresponding IWebView context with no result.
        /// </summary>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="functionName">
        /// function name
        /// </param>
        /// <param name="param">
        /// function parameters
        /// </param>
        void InvokeNoResult(string functionName, IWebView context, params IJavascriptObject[] param);

        /// <summary>
        /// Execute asynchronously the function in the coresponding IWebView context.
        /// </summary>
        /// <param name="functionName">
        /// function name
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="param">
        /// function parameters
        /// </param>
        /// <returns>
        /// task of the result of the execution
        ///</returns>
        Task<IJavascriptObject> InvokeAsync(string functionName, IWebView context, params IJavascriptObject[] param);

        /// <summary>
        /// Bind a function to a jaavscript object
        /// </summary>
        /// <param name="functionName">
        /// function name
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="action">
        /// action to run when the function is called. Action argument: 
        /// function name, this object, function parameters
        /// </param>
        void Bind(string functionName, IWebView context, Action<string, IJavascriptObject, IJavascriptObject[]> action);

        /// <summary>
        /// Bind a function to a javascript object
        /// </summary>
        /// <param name="functionName">
        /// function name
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="action">
        /// action to run when the function is called. Action agurment: 
        /// function name, function parameters
        /// </param>
        void Bind(string functionName, IWebView webView, Action<IJavascriptObject[]> action);

        /// <summary>
        /// Bind a function receiving one argument to a javascript object
        /// </summary>
        /// <param name="functionName">
        /// function name
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="action">
        /// action to run when the function is called. Action agurment: 
        /// function name, function first parameter
        /// </param>
        void BindArgument(string functionName, IWebView webView, Action<IJavascriptObject> action);

        /// <summary>
        /// Bind a function receiving two arguments to a javascript object
        /// </summary>
        /// <param name="functionName">
        /// function name
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="action">
        /// action to run when the function is called. Action agurment: 
        /// function name, function two first parameters
        /// </param>
        void BindArguments(string functionName, IWebView webView, Action<IJavascriptObject, IJavascriptObject> action);


        /// <summary>
        /// Bind a function receiving three arguments to a javascript object
        /// </summary>
        /// <param name="functionName">
        /// function name
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="action">
        /// action to run when the function is called. Action agurment: 
        /// function name, function three first parameters
        /// </param>
        void BindArguments(string functionName, IWebView webView, Action<IJavascriptObject, IJavascriptObject, IJavascriptObject> action);


        /// <summary>
        /// Bind a function receiving four arguments to a javascript object
        /// </summary>
        /// <param name="functionName">
        /// function name
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="action">
        /// action to run when the function is called. Action agurment: 
        /// function name, function four first parameters
        /// </param>
        void BindArguments(string functionName, IWebView webView, Action<IJavascriptObject, IJavascriptObject, IJavascriptObject, IJavascriptObject> action);

        /// <summary>
        /// Associates a value with the specified identifier.
        /// </summary>
        void SetValue(string attributeName, IJavascriptObject element, CreationOption option = CreationOption.None);

        /// <summary>
        /// For array set the valu for a given index.
        /// </summary>
        void SetValue(int index, IJavascriptObject element);

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
        /// Execute the function with arguments.
        /// </summary>
        /// <param name="webView">
        /// function context
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="parameters">
        /// function parameters
        /// </param>
        // <returns>
        /// the result of the execution
        ///</returns>
        IJavascriptObject ExecuteFunction(IWebView webView, IJavascriptObject context, params IJavascriptObject[] parameters);

        /// <summary>
        /// Execute the function with arguments and no result.
        /// </summary>
        /// <param name="webView">
        /// function context
        /// </param>
        /// <param name="context">
        /// function context
        /// </param>
        /// <param name="parameters">
        /// function parameters
        /// </param>
        void ExecuteFunctionNoResult(IWebView webView, IJavascriptObject context, params IJavascriptObject[] parameters);


        /// <summary>
        /// Returns the value with the specified identifier on success. Returns NULL
        /// if this method is called incorrectly or an exception is thrown.
        /// </summary>
        IJavascriptObject GetValue(string value);

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