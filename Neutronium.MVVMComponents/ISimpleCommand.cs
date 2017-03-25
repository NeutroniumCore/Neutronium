namespace Neutronium.MVVMComponents
{
    /// <summary>
    /// Command that can always be executed
    /// <seealso cref="ICommand"/>
    /// </summary>
    public interface ISimpleCommand
    {
        /// <summary>
        /// Execute the command with the corresponding argument
        /// </summary>
        /// <param name="argument"></param>
        void Execute(object argument);
    }
}
