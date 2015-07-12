using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefGlue.Window
{
    public interface IUIDispatcher
    {
        Task RunAsync(Action act);
    }
}
