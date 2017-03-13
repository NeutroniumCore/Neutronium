using System;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.WebBrowserEngine.Control
{
    /// <summary>
    /// Abstraction of HTML UI control (regardless of WPF or WindowsForm framework)
    /// </summary>
    public interface IWebBrowserWindowProvider : IDisposable
    {
        /// <summary>
        /// Return the IHTMLWindow
        /// </summary>
        IWebBrowserWindow HTMLWindow { get; }

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

        /// <summary>
        /// event send when debug tools is openong or closing
        /// </summary>
        event EventHandler<bool> DebugToolOpened;

        /// <summary>
        /// event send when WebBrowserWindowProvider is disposed
        /// </summary>
        event EventHandler OnDisposed;


    }
}
