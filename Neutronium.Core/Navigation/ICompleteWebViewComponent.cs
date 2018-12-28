using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Navigation
{
    /// <summary>
    /// WebView abstraction provided mainly to expose addictional API
    /// usefull on debug context.
    /// </summary>
    public interface ICompleteWebViewComponent : IWebViewComponent
    {
        /// <summary>
        /// open a HTML window with the given path and given action on WebViews:
        /// first is the current, second is the debug webView returning a
        /// disposable function called when window is closed
        /// </summary>
        /// <param name="path">
        /// Path to HTML
        /// </param>
        /// <param name="width">
        /// window width
        /// </param>
        /// <param name="height">
        /// window height
        /// </param>
        /// <param name="injectedCode">
        /// Inject code to the browser returning a disposable that will
        /// be disposed on window close.
        /// </param>
        void ShowHtmlWindow(string path, int width, int height, Func<IWebView, IDisposable> injectedCode);
    }
}
