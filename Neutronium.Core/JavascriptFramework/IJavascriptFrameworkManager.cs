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
        /// True if javascript framework as to create new javascript object to map 
        /// orginal ones. False if it use standard objects.
        /// </summary>
        bool IsMappingObject { get; }

        /// <summary>
        /// Create an IJavascriptViewModelManager from webview and IJavascriptObserver listener object
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
        /// Returns true if the javascript framework manager support
        /// addintional Vm debug capability. If so beguVM command will be enabled.
        /// </summary>
        bool IsSupportingVmDebug { get; }

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
        void DebugVm(Action<string> runJavascript, Action<string, int, int, Func<IWebView, IWebView, IDisposable>> openNewWindow);

        /// <summary>
        /// return main javascript debug including framework code
        /// </summary>
        /// <param name="debugContext">
        /// True if debug mode is activated
        /// </param>
        string GetMainScript(bool debugContext);

        /// <summary>
        /// Get DebugToolsUI framework specific UI if any
        /// <seealso cref="DebugToolsUI"/>
        /// </summary>
        DebugToolsUI DebugToolsUI { get; }
    }
}
