using System;
using Xilium.CefGlue;

namespace MVVM.CEFGlue
{
    public interface IHTMLBinding : IDisposable
    {
        CefV8Value JSRootObject { get; }

        object Root { get; }
    }
}
