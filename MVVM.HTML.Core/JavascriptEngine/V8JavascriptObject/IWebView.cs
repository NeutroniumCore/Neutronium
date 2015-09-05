using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.HTML.Core.Window;


namespace MVVM.HTML.Core.V8JavascriptObject
{
    /// <summary>
    /// Javacsript Window interaction
    /// </summary>
    public interface IWebView : IDispatcher
    {
        /// <summary>
        /// Dispatch on a safe thread. If the caller is already on the safe
        /// thread the action is not excecute immediately
        /// </summary>
        /// <param name="act">
        /// Action to be executed
        /// </param>
        /// <returns>
        /// the corresponding task
        ///</returns>
        Task DispatchAsync(Action act);

        /// <summary>
        /// Get the window object
        /// </summary>
        IJavascriptObject GetGlobal();

        /// <summary>
        /// Get the javascript bsic converter
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
        /// <returns>
        /// true if code run without error
        ///</returns>
        bool Eval(string code, out IJavascriptObject res);

        /// <summary>
        ///  Evaluate a javascript code with result
        /// </summary>
        void ExecuteJavaScript(string code);
    }
}
