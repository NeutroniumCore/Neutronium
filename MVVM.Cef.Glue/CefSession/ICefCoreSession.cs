using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xilium.CefGlue;
using MVVM.HTML.Core.Window;

namespace MVVM.Cef.Glue.CefSession
{
    public interface ICefCoreSession : IDisposable
    {
        //IDispatcher UIDispatcher { get; }

        MVVMCefApp CefApp  {  get ;  }

        CefSettings CefSettings { get; }
    }
}
