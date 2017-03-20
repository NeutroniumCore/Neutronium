namespace Neutronium.Core.Navigation
{
    /// <summary>
    /// interface to be implemented by ViewModel in order to received
    /// a reference to <see cref="INavigationSolver"/> instance during navigation
    /// </summary>
    public interface INavigable
    {
        /// <summary>
        /// Reference to <see cref="INavigationSolver"/>. Set during navigation by navigation solver.
        /// </summary>
        INavigationSolver Navigation { get; set; }
    }
}
