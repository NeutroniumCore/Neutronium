using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine;


namespace MVVM.HTML.Core.Window
{
    /// <summary>
    /// HTML Window abstraction
    /// </summary>
    public interface IHTMLWindow
    {
        /// <summary>
        /// Get the main frame logic
        /// </summary>
        IWebView MainFrame { get; }

        /// <summary>
        /// Navigate to the specified path
        /// </summary>
        /// <param name="path">
        /// Path to navigate to
        /// </param>
        void NavigateTo(string path);

        /// <summary>
        /// true if the content is loaded and ready
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// event fired when the window is loaded
        /// </summary>
        event EventHandler<LoadEndEventArgs> LoadEnd;

        /// <summary>
        /// event fired when the console log is called in the browser
        /// </summary>
        event EventHandler<ConsoleMessageArgs> ConsoleMessage;

        /// <summary>
        /// get the browser dispatcher
        /// </summary>
        IDispatcher GetDispatcher();
    }
}
