using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefGlueHelper
{
    internal class CefV8Handler_Function : CefV8Handler
    {
        private Func<string, CefV8Value, CefV8Value[],Tuple<CefV8Value, string>> _Function;
        internal CefV8Handler_Function(Func<string, CefV8Value, CefV8Value[], Tuple<CefV8Value, string>> iFunction)
        {
            _Function = iFunction;
        }
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            var res = _Function(name, obj, arguments);
            returnValue = res.Item1;
            exception = res.Item2;
            return true;
        }
    }


    internal class CefV8Handler_Simple_Function : CefV8Handler
    {
        private Func<CefV8Value, CefV8Value[], Tuple<CefV8Value, string>> _Function;
        internal CefV8Handler_Simple_Function(Func<CefV8Value, CefV8Value[], Tuple<CefV8Value, string>> iFunction)
        {
            _Function = iFunction;
        }
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            var res = _Function(obj, arguments);
            returnValue = res.Item1;
            exception = res.Item2;
            return true;
        }
    }
}
