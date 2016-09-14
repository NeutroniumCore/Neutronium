using System;

namespace Neutronium.Core.WebBrowserEngine.Window
{
    /// <summary>
    /// HTML Window with extended capacity
    /// </summary>
    public interface IModernWebBrowserWindow : IWebBrowserWindow
    {
        /// <summary>
        /// event fired when script are ready to be executed
        /// but external javascript have not been executed yet
        /// </summary>
        event EventHandler<BeforeJavascriptExcecutionArgs> BeforeJavascriptExecuted;
    }
}
