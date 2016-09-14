using System;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.CefSession
{
    public interface ICefCoreSession : IDisposable
    {
        NeutroniumCefApp CefApp  {  get ;  }

        CefSettings CefSettings { get; }
    }
}
