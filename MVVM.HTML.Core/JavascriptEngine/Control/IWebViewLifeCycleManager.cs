using MVVM.HTML.Core.JavascriptEngine.Window;

namespace MVVM.HTML.Core.JavascriptEngine.Control
{
    /// <summary>
    /// HTMLWindow provider
    /// </summary>
    public interface IWebViewLifeCycleManager
    {
        /// <summary>
        /// Create the HTMLWindowProvider
        /// </summary>
        IHTMLWindowProvider Create();

        /// <summary>
        /// Return the corresponding UI dispatcher
        /// </summary>
        IDispatcher GetDisplayDispatcher();
    }
}
