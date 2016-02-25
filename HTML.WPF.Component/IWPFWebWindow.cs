using System;
using System.Windows;
using System.Windows.Input;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace HTML_WPF.Component
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
    }
}
