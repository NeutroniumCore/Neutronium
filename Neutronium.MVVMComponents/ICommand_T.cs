using System;

namespace Neutronium.MVVMComponents
{
    /// <summary>
    /// Command that receive a typed argument
    /// <seealso cref="ICommand"/>
    /// </summary>
    public interface ICommand<in T>
    {
        /// <summary>
        /// Execute the command with the corresponding argument
        /// </summary>
        /// <param name="argument"></param>
        void Execute(T argument);

        ///<summary>
        ///     Occurs when changes occur that affect whether or not the command should execute.
        ///</summary>
        event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Defines the method that determines whether the command can execute in its current
        ///     state.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command. If the command does not require data to be passed,
        ///     this object can be set to null.
        /// </param>
        /// <returns>
        ///     true if this command can be executed; otherwise, false.
        /// </returns>
        bool CanExecute(T parameter);
    }
}
