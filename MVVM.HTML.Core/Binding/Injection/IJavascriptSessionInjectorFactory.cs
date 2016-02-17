using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Binding
{
    /// <summary>
    /// IJavascriptSessionInjector factory
    /// </summary>
    public interface IJavascriptSessionInjectorFactory
    {
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
        /// true if the injector returns a valid devug script
        /// </summary>
        bool HasDebugScript();
    }
}
