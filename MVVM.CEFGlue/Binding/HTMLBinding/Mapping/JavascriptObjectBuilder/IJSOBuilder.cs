using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.HTMLBinding
{
    public interface IJSOBuilder
    {
        CefV8Value CreateJSO();

        uint GetID(CefV8Value iJSObject);

        bool HasRelevantId(CefV8Value iJSObject);
    }  
}
