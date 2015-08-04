using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xilium.CefGlue;

using CefGlue.Window;

namespace CefGlue.Window
{
    public interface ICefGlueWindow
    {
        CefFrame MainFrame { get; }

        bool IsLoaded { get; }

        event EventHandler<LoadEndEventArgs> LoadEnd;

        IDispatcher GetDispatcher();
    }
}
