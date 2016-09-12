using System;
using Neutronium.Core;
using Neutronium.Core.JavascriptUIFramework;

namespace HTML_WPF.Component
{
    /// <summary>
    /// Singleton object that allows you to register IWPFWebWindowFactory
    /// to the current session.
    /// </summary>
    public interface IHTMLEngineFactory : IDisposable
    {
        /// <summary>
        /// Find a IWPFWebWindowFactory by name.
        /// This method is called internally by HTLM_WPF.Component controls
        /// </summary>
        /// <param name="engineName">
        /// the name of the factory to be found
        /// </param>
        /// <returns>
        /// IWPFWebWindowFactory registered with a similar name.
        ///</returns>
        IWPFWebWindowFactory ResolveJavaScriptEngine(string engineName);

        /// <summary>
        /// register a IWPFWebWindowFactory using its Name property
        /// </summary>
        /// <param name="wpfWebWindowFactory">
        /// IWPFWebWindowFactory to be registered
        /// </param>
        void RegisterHTMLEngine(IWPFWebWindowFactory wpfWebWindowFactory);

        /// <summary>
        /// Find a IJavascriptUiFrameworkManager by name.
        /// This method is called internally by HTLM_WPF.Component controls
        /// </summary>
        /// <param name="frameworkName">
        /// the name of the factory to be found
        /// </param>
        /// <returns>
        /// IJavascriptUiFrameworkManager registered with a similar name.
        ///</returns>
        IJavascriptUiFrameworkManager ResolveJavaScriptFramework(string frameworkName);

        /// <summary>
        /// register a IJavascriptUiFrameworkManager using its Name property
        /// </summary>
        /// <param name="javascriptUiFrameworkManager">
        /// IJavascriptUiFrameworkManager to be registered
        /// </param>
        void RegisterJavaScriptFramework(IJavascriptUiFrameworkManager javascriptUiFrameworkManager);

        /// <summary>
        /// get or set WebSessionLogger
        /// </summary>
        IWebSessionLogger WebSessionLogger { get; set; }
    }
}
