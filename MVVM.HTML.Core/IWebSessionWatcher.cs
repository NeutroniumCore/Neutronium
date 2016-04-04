using System;

namespace MVVM.HTML.Core
{
    /// <summary>
    /// Interface to implement to receive event from browser
    /// </summary>
    public interface IWebSessionWatcher
    {
        /// <summary>
        /// called on critical event 
        /// </summary>
        void LogCritical(string information);

        /// <summary>
        /// called on each consolo log called by browser 
        /// </summary>
        void LogBrowser(string iInformation);

        /// <summary>
        /// called in case of browser critical error
        /// </summary>
        void OnSessionError(Exception exception, Action cancel);
    }
}
