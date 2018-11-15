using System;

namespace Neutronium.Core.WebBrowserEngine.Window
{
    /// <summary>
    /// Abstraction of a dispatcher designed for the Ui thread
    /// </summary>
    public interface IUiDispatcher: IDispatcher
    {
        /// <summary>
        /// Run on a safe thread the corresponding action
        /// in a none-blocking manner.
        /// The action should be given a binding priority
        /// which is tipically slighty lower than default
        /// priority.
        /// No task is return for performance optimization.
        /// This action should not be executed immediatly
        /// even if the execution is currently in context.
        /// </summary>
        /// <param name="act">
        /// Action to be executed
        /// </param>
        void DispatchWithBindingPriority(Action act);
    }
}
