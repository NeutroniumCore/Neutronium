using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;

namespace Neutronium.Core.JavascriptFramework
{
    /// <summary>
    /// IJavascriptFrameworkManager factory
    /// </summary>
    public interface IJavascriptFrameworkManager
    {
        /// <summary>
        /// Get the name of unferlying javascript framework
        /// </summary>
        string FrameworkName { get; }

        /// <summary>
        /// Get the version of unferlying javascript framework
        /// </summary>
        string FrameworkVersion { get; }

        /// <summary>
        /// name of the javascript C# bridge engine
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Create an IJavascriptViewModelManager from webview and IJavascriptObserver listner object
        /// </summary>
        /// <param name="webView">
        /// IWebView
        /// </param>
        /// <param name="listener">
        /// listener to call on changes 
        /// </param>
        /// <param name="logger">
        /// logger
        /// </param>
        /// <returns>
        /// the newly created IJavascriptViewModelManager
        ///</returns>
        IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger, bool debugMode);

        /// <summary>
        /// Run debug Vm tool
        /// </summary>
        /// <param name="runJavascript">
        /// execute javascript in the current window
        /// </param>
        /// <param name="openNewWindow">
        /// open a HTML window with the given path and given action on WebViews:
        /// first is the current, second is the debug webview returning a
        /// disposable function called when window is closed
        /// </param>
        void DebugVm(Action<string> runJavascript, Action<string, Func<IWebView, IWebView, IDisposable>> openNewWindow);

        /// <summary>
        /// return main javascript debug including framework code
        /// </summary>
        /// <param name="debugContext">
        /// True if debug mode is activated
        /// </param>
        string GetMainScript(bool debugContext);

        /// <summary>
        /// return relative path to toolbar HTML file if any
        /// null otherwise.
        /// </summary>
        string DebugToolbarRelativePath { get;  }

        /// <summary>
        /// return relative path to about screen HTML file if any
        /// null otherwise.
        /// </summary>
        string AboutRelativePath { get; }
    }
}
