using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_WPF.Component
{
    /// <summary>
    /// IWPFWebWindow factory: abstraction of a WPF implementation of an HTML Browser
    /// </summary>
    public interface IWPFWebWindowFactory : IDisposable
    {
        /// <summary>
        /// value of the Engine Name
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
