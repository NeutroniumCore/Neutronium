using MVVM.CEFGlue.CefGlueHelper;
using System;
using Xilium.CefGlue;

namespace MVVM.CEFGlue
{
    public interface IHTMLBinding : IDisposable
    {
        CefV8CompleteContext Context { get; }

        CefV8Value JSRootObject { get; }

        object Root { get; }
    }
}
