using System;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.CefGlueHelper
{
    internal class CefV8Handler_Action : CefV8Handler
    {
        private Action< string, CefV8Value, CefV8Value[]> _Action;
        internal CefV8Handler_Action( Action<string, CefV8Value, CefV8Value[]> iAction)
        {
            _Action = iAction;
        }
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            returnValue = null;
            exception = null;
            _Action(name, obj, arguments);
            return false;
        }
    }
}
