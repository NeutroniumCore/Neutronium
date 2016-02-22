using System;

namespace HTML_WPF.Component
{
    /// <summary>
    /// IWPFWebWindow factory: abstraction of a WPF implementation of an HTML Browser
    /// </summary>
    public interface IWPFWebWindowFactory : IDisposable
    {
        /// <summary>
        /// Get the name and version of unferlying javascript engine
        /// </summary>
        string EngineName { get; }

        /// <summary>
        /// value of the .Net Glue to javascript engine
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Create a new IWPFWebWindow 
        /// </summary>
        /// <returns>
        /// a new IWPFWebWindow
        ///</returns>
        IWPFWebWindow Create();

        /// <summary>
        /// Get the localhost port allowing HTML debugging
        /// </summary>
        /// <returns>
        /// Debugging port, null if not alloed
        ///</returns>
        Nullable<int> GetRemoteDebuggingPort();
    }
}
