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
        /// Get the name of underlying javascript engine
        /// </summary>
        string EngineName { get; }

        /// <summary>
        /// Get the version of underlying javascript engine
        /// </summary>
        string EngineVersion { get; }

        /// <summary>
        /// Get the javascript engine environment information
        /// including Platform build if nay
        /// </summary>
        string Environment { get; }

        /// <summary>
        /// value of the .Net glue framework to javascript engine
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Create a new IWPFWebWindow 
        /// </summary>
        ///<param name="useNeutroniumSettings">
        /// indicates if browser should use dedicated neutronium settings
        /// </param>
        /// <returns>
        /// a new IWPFWebWindow <see cref="IWPFWebWindow"/>
        ///</returns>
        IWPFWebWindow Create(bool useNeutroniumSettings);

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
