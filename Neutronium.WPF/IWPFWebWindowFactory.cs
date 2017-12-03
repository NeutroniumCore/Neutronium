using System;
using Neutronium.Core;

namespace Neutronium.WPF
{
    /// <summary>
    /// IWPFWebWindow factory: abstraction of a WPF implementation of an HTML Browser
    /// </summary>
    public interface IWPFWebWindowFactory : IDisposable
    {
        /// <summary>
        /// Get the name of unferlying javascript engine
        /// </summary>
        string EngineName { get; }

        /// <summary>
        /// Get the version of unferlying javascript engine
        /// </summary>
        string EngineVersion { get; }

        /// <summary>
        /// value of the .Net glue framework to javascript engine
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Create a new IWPFWebWindow 
        /// </summary>
        /// <returns>
        /// a new IWPFWebWindow <see cref="IWPFWebWindow"/>
        ///</returns>
        IWPFWebWindow Create();

        /// <summary>
        /// get IsModern value
        /// </summary>
        /// <returns>
        /// true if engine create a IWPFWebWindow which HTMLWindow
        /// is a IModernWebBrowserWindow
        ///</returns>
        bool IsModern { get; }

        /// <summary>
        /// get or set WebSessionLogger
        /// </summary>
        IWebSessionLogger WebSessionLogger { get; set; }
    }
}
