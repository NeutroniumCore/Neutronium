using System;

namespace Neutronium.MVVMComponents
{
    /// <summary>
    /// Command that receive no argument
    /// <seealso cref="ICommand"/>
    /// </summary>
    public interface ICommandWithoutParameter
    {
        /// <summary>
        /// Execute the command
        /// </summary>
        void Execute();

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        event EventHandler CanExecuteChanged;

        /// <summary>
        ///  Determines whether the command can execute in its current
        ///  state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        bool CanExecute { get; }
    }
}
