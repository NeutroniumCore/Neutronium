using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.HTMLBinding
{
    public interface IJSCSGlue
    {
        CefV8Value JSValue { get; }

        object CValue { get;}

        JSCSGlueType Type { get; }

        IEnumerable<IJSCSGlue> GetChildren();

        void BuilString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed);
    }
}
