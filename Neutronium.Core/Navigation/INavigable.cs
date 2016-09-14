namespace Neutronium.Core.Navigation
{
    public interface INavigable
    {
        INavigationSolver Navigation { get; set; }
    }
}
