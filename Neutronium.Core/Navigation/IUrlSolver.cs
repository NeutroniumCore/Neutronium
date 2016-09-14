using System;

namespace Neutronium.Core.Navigation
{
    public interface IUrlSolver
    {
        Uri Solve(object viewModel, string id = null);
    }
}
