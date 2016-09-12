using System;
using Neutronium.Core.JavascriptEngine.Window;

namespace Neutronium.Core.JavascriptEngine.Control
{
    /// <summary>
    /// Abstraction of HTML UI control (regardless of WPF or WindowsForm framework)
    /// </summary>
    public interface IHTMLWindowProvider : IDisposable
    {
        /// <summary>
        /// Return the IHTMLWindow
        /// </summary>
        IHTMLWindow HTMLWindow { get; }

        /// <summary>
        /// Return the UI dispatcher
        /// </summary>
        IDispatcher UIDispatcher { get; }

        /// <summary>
        /// Show the control on the screen
        /// </summary>
        void Show();

        /// <summary>
        /// Hide the control
        /// </summary>
        void Hide();

        /// <summary>
        /// Called when user request debug tools
        /// returns false if this options is not handled, true otherwise
        /// </summary>
        bool OnDebugToolsRequest();

        /// <summary>
        /// Close debug tools window if possible
        /// </summary>
        void CloseDebugTools();
    }
}
