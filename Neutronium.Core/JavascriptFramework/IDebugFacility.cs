using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.JavascriptFramework
{
    /// <summary>
    /// interface used by javascript framework debug vm command
    /// </summary>
    public interface IDebugFacility
    {
        /// <summary>
        /// Execute javascript in the current window
        /// </summary>
        /// <param name="code">
        /// Code to be executed
        /// </param>
        void RunJavascript(string code);

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
        /// <param name="injectCode">
        /// Receive main current webView and debug webView and returns an Disposable that
        /// will be called on closing the debug window
        /// </param>
        void OpenNewWindow(string path, int width, int height, Func<IWebView, IWebView, IDisposable> injectCode);
    }
}
