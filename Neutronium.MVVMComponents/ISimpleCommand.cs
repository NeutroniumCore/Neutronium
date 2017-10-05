namespace Neutronium.MVVMComponents
{
    /// <summary>
    /// Command that can always be executed
    /// <seealso cref="ICommand"/>
    /// </summary>
    public interface ISimpleCommand
    {
        /// <summary>
        /// Execute the command without argument
        /// </summary>
        void Execute();
    }
}
