using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.HTML.Core
{
    public interface INavigable
    {
        INavigationSolver Navigation { get; set; }
    }
}
