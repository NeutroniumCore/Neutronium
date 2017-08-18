using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.WebBrowserEngine.JavascriptObject
{
    /// <summary>
    /// Javascript Window interaction
    /// </summary>
    public interface IWebView : IDispatcher
    {
        /// <summary>
        /// Get the window object
        /// </summary>
        IJavascriptObject GetGlobal();

        /// <summary>
        /// Get the javascript basic converter
        /// </summary>
        IJavascriptObjectConverter Converter { get; }

        /// <summary>
        /// Get the javascript factory
        /// </summary>
        IJavascriptObjectFactory Factory { get; }

        /// <summary>
        /// Evaluate a javascript code synchroneousely and return a result
        /// </summary>
        /// <param name="code">
        /// javascript code to be executed
        /// </param>
        /// <param name="res">
        /// javascript object returned by the code
        /// </param>
        /// <returns>
        /// true if code run without error
        ///</returns>
        bool Eval(string code, out IJavascriptObject res);

        /// <summary>
        ///  Evaluate a javascript code without result
        /// </summary>
        void ExecuteJavaScript(string code);

        /// <summary>
        ///  true if object bulk creation is supported
        ///  false for awesomium only
        /// </summary>
        bool AllowBulkCreation { get; }

        /// <summary>
        ///  return the max number of arguments 
        ///  javaScript functions can accept
        ///  https://stackoverflow.com/questions/22747068/is-there-a-max-number-of-arguments-javascript-functions-can-accept
        ///  http://jsfiddle.net/dede89/uxd7bycx/
        /// </summary>
        int MaxFunctionArgumentsNumber { get; }
    }
}
