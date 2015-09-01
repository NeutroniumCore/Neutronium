using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_WPF.Component
{
    public interface IWPFWebWindowFactory : IDisposable
    {
        string Name { get; }

        IWPFWebWindow Create();

        Nullable<int> GetRemoteDebuggingPort();
    }
}
