using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.WebBrowserEngine.Control
{
    /// <summary>
    /// HTMLWindow provider
    /// </summary>
    public interface IWebViewLifeCycleManager
    {
        /// <summary>
        /// Create the HTMLWindowProvider
        /// </summary>
        IWebBrowserWindowProvider Create();

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
