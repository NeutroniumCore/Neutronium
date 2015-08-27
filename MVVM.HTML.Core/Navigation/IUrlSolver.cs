using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.HTML.Core
{
    public interface IUrlSolver
    {
        Uri Solve(object iViewModel, string Id = null);
    }
}
