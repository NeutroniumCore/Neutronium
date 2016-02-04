using System;

namespace HTML_WPF.Component
{
    /// <summary>
    /// Singleton object that allows you to register IWPFWebWindowFactory
    /// to the current session.
    /// </summary>
    public interface IHTMLEngineFactory : IDisposable
    {
        /// <summary>
        /// Find a IWPFWebWindowFactory by name.
        /// This method is called internally by HTLM_WPF.Component controls
        /// </summary>
        /// <param name="EngineName">
        /// the name of the factory to be found
        /// </param>
        /// <returns>
        /// IWPFWebWindowFactory registered with a similar name.
        ///</returns>
        IWPFWebWindowFactory Resolve(string EngineName);

        /// <summary>
        /// register a IWPFWebWindowFactory using its Name property
        /// </summary>
        /// <param name="value">
        /// IWPFWebWindowFactory to be registered
        /// </param>
        void Register(IWPFWebWindowFactory iWPFWebWindowFactory);

    }
}
