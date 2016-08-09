using System;

namespace MVVM.HTML.Core.Navigation
{
    public interface IUrlSolver
    {
        Uri Solve(object viewModel, string id = null);
    }
}
