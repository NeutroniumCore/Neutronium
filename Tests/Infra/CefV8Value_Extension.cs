using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;
using MVVM.CEFGlue.CefGlueImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.Test.Infra
{
    public static class CefV8Value_Extension
    {
        public static IJavascriptObject Convert(this CefV8Value value)
        {
            return new CefV8_JavascriptObject(value);
        }

        public static CefV8Value Convert(this IJavascriptObject value)
        {
            return (value as CefV8_JavascriptObject).RawValue;
        }
    }
}
