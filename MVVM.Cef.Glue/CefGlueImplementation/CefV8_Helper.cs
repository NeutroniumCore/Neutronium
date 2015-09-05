using MVVM.HTML.Core.V8JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue
{
    public static class CefV8_Helper
    {
        public static CefV8Value Convert(this IJavascriptObject value)
        {
            return (value as CefV8_JavascriptObject).RawValue;
        }
    }
}
