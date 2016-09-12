using System;
using System.Windows;
using System.Windows.Input;
using Neutronium.Core.JavascriptEngine.Window;

namespace Neutronium.WPF
{
    /// <summary>
    /// Abstraction of a WPF implementation of an HTML Browser
    /// </summary>
    public interface IWPFWebWindow : IDisposable
    {
        /// <summary>
        /// Get the browser HTMLWindow
        /// </summary>
        IHTMLWindow HTMLWindow { get; }

        /// <summary>
        /// Inject a key event to the browser
        /// </summary>
        /// <param name="keyToInject">
        /// the key value to inject to the browser
        /// </param>
        void Inject(Key keyToInject);

        /// <summary>
        /// Get the UIElement representing the browser
        /// </summary>
        UIElement UIElement { get; }

        /// <summary>
        /// True if UIElement is always topmost as it is the
        /// case when it is an embedded WindowsForm Element 
        /// </summary>
        bool IsUIElementAlwaysTopMost { get; }

        ///// <summary>
        ///// Called when user request debug tools
        ///// returns false if this options is not handled, true otherwise
        ///// </summary>
        bool OnDebugToolsRequest();

        /// <summary>
        /// Close debug tools window if possible
        /// </summary>
        void CloseDebugTools();
    }
}
