using System;

namespace Neutronium.Core.JavascriptEngine.Window
{
    /// <summary>
    /// HTML Window with extended capacity
    /// </summary>
    public interface IHTMLModernWindow : IHTMLWindow
    {
        /// <summary>
        /// event fired when script are ready to be executed
        /// but external javascript have not been executed yet
        /// </summary>
        event EventHandler<BeforeJavascriptExcecutionArgs> BeforeJavascriptExecuted;
    }
}
