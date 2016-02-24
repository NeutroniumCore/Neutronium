using System;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.JavascriptUIFramework
{
    /// <summary>
    /// IJavascriptSessionInjector factory
    /// </summary>
    public interface IJavascriptSessionInjectorFactory
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
        /// Create an IJavascriptSessionInjector from webview and IJavascriptChangesObserver
        /// </summary>
        /// <param name="webView">
        /// IWebView
        /// </param>
        /// <param name="javascriptObserver">
        /// IJavascriptChangesObserver 
        /// </param>
        /// <returns>
        /// the newly created IJavascriptSessionInjector
        ///</returns>
        IJavascriptSessionInjector CreateInjector(IWebView webView, IJavascriptChangesObserver javascriptObserver);

        /// <summary>
        /// return javascript debug script to allow interactive debug
        /// of view model bound to the view
        /// </summary>
        string GetDebugScript();

        /// <summary>
        /// return main javascript debug including framework code
        /// </summary>
        string GetMainScript();

        /// <summary>
        /// true if the injector returns a valid debug script
        /// </summary>
        bool HasDebugScript();
    }
}
