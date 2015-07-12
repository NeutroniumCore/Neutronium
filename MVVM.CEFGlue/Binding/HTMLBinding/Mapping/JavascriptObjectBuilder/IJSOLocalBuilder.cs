using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.HTMLBinding
{
    public interface IJSOLocalBuilder : IJSOBuilder
    {
        CefV8Value CreateDate(DateTime dt);

        CefV8Value CreateEnum(Enum ienum);

        CefV8Value CreateNull();
    }
}
