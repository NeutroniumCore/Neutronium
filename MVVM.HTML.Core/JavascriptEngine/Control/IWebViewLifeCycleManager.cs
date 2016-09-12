using Neutronium.Core.JavascriptEngine.Window;

namespace Neutronium.Core.JavascriptEngine.Control
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

        /// <summary>
        /// True if the window is under debug 
        /// </summary>
        bool DebugContext { get; }
    }
}
