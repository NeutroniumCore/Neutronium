using System;

namespace MVVM.HTML.Core
{
    public interface IUrlSolver
    {
        Uri Solve(object iViewModel, string Id = null);
    }
}
