using System;
using MVVM.HTML.Core.JavascriptUIFramework;

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
        void Register(IWPFWebWindowFactory wpfWebWindowFactory);


        /// <summary>
        /// Find a IJavascriptSessionInjectorFactory by name.
        /// This method is called internally by HTLM_WPF.Component controls
        /// </summary>
        /// <param name="frameworkName">
        /// the name of the factory to be found
        /// </param>
        /// <returns>
        /// IJavascriptSessionInjectorFactory registered with a similar name.
        ///</returns>
        IJavascriptSessionInjectorFactory ResolveJavaScriptFramework(string frameworkName);

        /// <summary>
        /// register a IJavascriptSessionInjectorFactory using its Name property
        /// </summary>
        /// <param name="javascriptSessionInjectorFactory">
        /// IJavascriptSessionInjectorFactory to be registered
        /// </param>
        void Register(IJavascriptSessionInjectorFactory javascriptSessionInjectorFactory);

    }
}
