using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.JavascriptFramework
{
    /// <summary>
    /// IJavascriptFrameworkManager factory
    /// </summary>
    public interface IJavascriptFrameworkManager
    {
        /// <summary>
        /// Get the name and version of unferlying javascript framework
        /// </summary>
        string FrameworkName { get; }

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
        IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger);

        /// <summary>
        /// return javascript debug script to allow interactive debug
        /// of view model bound to the view
        /// </summary>
        string GetDebugScript();

        /// <summary>
        /// return javascript script that toogle the display of debug tool
        /// </summary>
        string GetDebugToogleScript();

        /// <summary>
        /// return main javascript debug including framework code
        /// </summary>
        /// <param name="debugContext">
        /// True if debug mode is activated
        /// </param>
        string GetMainScript(bool debugContext);

        /// <summary>
        /// true if the injector returns a valid debug script
        /// </summary>
        bool HasDebugScript();


        /// <summary>
        /// relative path to toolbar HTML file to debug toolbar implementation if any
        /// null otherwise.
        /// </summary>
        string DebugToolbaRelativePath { get;  }
    }
}
