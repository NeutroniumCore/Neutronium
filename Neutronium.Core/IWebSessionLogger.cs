using System;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core {
    /// <summary>
    /// Interface to implement to receive event from browser
    /// </summary>
    public interface IWebSessionLogger
    {
        /// <summary>
        /// Called for debug logging
        /// </summary>
        void Debug(Func<string> information);

        /// <summary>
        /// Called for debug logging
        /// </summary>
        void Debug(string information);

        /// <summary>
        /// Called for information logging
        /// </summary>
        void Info(string information);

        /// <summary>
        /// called for information logging 
        /// </summary>
        void Info(Func<string> information);

        /// <summary>
        /// Called for warning 
        /// </summary>
        void Warning(string information);

        /// <summary>
        /// Called for warning 
        /// </summary>
        void Warning(Func<string> information);

        /// <summary>
        /// Called on critical event 
        /// </summary>
        void Error(string information);

        /// <summary>
        /// Called on critical event 
        /// </summary>
        void Error(Func<string> information);

        /// <summary>
        /// Called on each console log called by browser 
        /// </summary>
        void LogBrowser(ConsoleMessageArgs information, Uri url);

        /// <summary>
        /// Called in case of browser critical error
        /// </summary>
        /// <param name="exception">
        /// Exception responsible for the error
        /// </param>
        /// <param name="cancel">
        /// Action to be called to cancel browser closing,
        /// has an effect only with Awesomium
        /// </param>
        void WebBrowserError(Exception exception, Action cancel);
    }
}
