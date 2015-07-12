using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.HTMLBinding
{
    public interface IGlobalBuilder : IJSOBuilder
    {
        uint CreateAndGetID(CefV8Value iJSObject);
    }
}
