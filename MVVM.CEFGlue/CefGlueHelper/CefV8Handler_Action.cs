using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefGlueHelper
{
    internal class CefV8Handler_Action : CefV8Handler
    {
        private Action<CefV8Context,string, CefV8Value, CefV8Value[]> _Action;
        private CefV8Context _CefV8Context;
        internal CefV8Handler_Action(CefV8Context iCefV8Context, Action<CefV8Context,string, CefV8Value, CefV8Value[]> iAction)
        {
            _CefV8Context = iCefV8Context;
            _Action = iAction;
        }
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            returnValue = null;
            exception = null;
            _Action(_CefV8Context,name, obj, arguments);
            return false;
        }
    }

    internal class CefV8Handler_Simple_Action : CefV8Handler
    {
        private Action<CefV8Context, CefV8Value, CefV8Value[]> _Action;
        private CefV8Context _CefV8Context;
        internal CefV8Handler_Simple_Action(CefV8Context iCefV8Context, Action<CefV8Context, CefV8Value, CefV8Value[]> iAction)
        {
            _CefV8Context = iCefV8Context;
            _Action = iAction;
        }
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            returnValue = null;
            exception = null;
            _Action(_CefV8Context,obj, arguments);
            return false;
        }
    }
}
