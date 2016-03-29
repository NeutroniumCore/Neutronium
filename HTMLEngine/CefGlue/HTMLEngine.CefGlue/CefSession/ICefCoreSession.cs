using System;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.CefSession
{
    public interface ICefCoreSession : IDisposable
    {
        MVVMCefApp CefApp  {  get ;  }

        CefSettings CefSettings { get; }
    }
}
