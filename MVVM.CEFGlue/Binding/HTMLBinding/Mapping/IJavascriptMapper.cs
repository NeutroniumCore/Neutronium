using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.HTMLBinding
{
    internal interface IJavascriptMapper
    {
        void RegisterFirst(CefV8Value iRoot);

        void RegisterMapping(CefV8Value iFather, string att, CefV8Value iChild);

        void RegisterCollectionMapping(CefV8Value iFather, string att, int index, CefV8Value iChild);

        void End(CefV8Value iRoot);
    }
}
