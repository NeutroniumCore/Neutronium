using System;
using Xilium.CefGlue;

namespace HTMLEngine.CefGlue.CefSession
{
    public interface ICefCoreSession : IDisposable
    {
        MVVMCefApp CefApp  {  get ;  }

        CefSettings CefSettings { get; }
    }
}
