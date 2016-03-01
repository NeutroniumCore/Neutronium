using System;
using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace MVVM.HTML.Core.JavascriptEngine.JavascriptObject
{
    /// <summary>
    /// Javacsript Window interaction
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
    }
}
