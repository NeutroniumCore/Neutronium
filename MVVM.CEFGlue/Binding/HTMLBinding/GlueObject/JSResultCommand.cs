using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xilium.CefGlue;

using MVVM.CEFGlue.CefGlueHelper;
using MVVM.Component;

namespace MVVM.CEFGlue.HTMLBinding
{
    public class JSResultCommand : GlueBase, IJSObservableBridge
    {
        private IResultCommand _JSResultCommand;
        private CefV8Context _CefV8Context;
        public JSResultCommand(CefV8Context ijsobject, IJSOBuilder builder, IResultCommand icValue)
        {
            _CefV8Context = ijsobject;
            _JSResultCommand = icValue;
            JSValue = builder.CreateJSO();    
        }

        public CefV8Value JSValue { get; private set; }

        private CefV8Value _MappedJSValue;

        public CefV8Value MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(CefV8Value ijsobject, IJSCBridgeCache mapper)
        {
            _MappedJSValue = ijsobject;
            //((JSObject)_MappedJSValue).Bind("Execute", false, (o, e) => Execute(e, mapper));
            _MappedJSValue.Bind("Execute", _CefV8Context,(c, o, e) => Execute(e, mapper));
        }

        private object Convert(IJSCBridgeCache mapper, CefV8Value value)
        {
            var found = mapper.GetCachedOrCreateBasic(value, null);
            return (found != null) ? found.CValue : null;
        }

        private object GetArguments(IJSCBridgeCache mapper, CefV8Value[] e)
        {
            return (e.Length == 0) ? null : Convert(mapper, e[0]);
        }

        private void SetResult(CefV8Value[] e, IJSCBridgeCache bridge, Task<object> resulttask)
        {
            _CefV8Context.RunInContextAsync (() =>
                 {
                     if (e.Length < 2)
                         return;

                    

                     _CefV8Context.Enter(); 
                     
                     CefV8Value promise = e[1];
                     if (!resulttask.IsFaulted)
                     {
                         bridge.RegisterInSession(resulttask.Result, (bridgevalue) =>
                         {
                             promise.InvokeAsync("fullfill", _CefV8Context, bridgevalue.GetJSSessionValue());
                         });
                     }
                     else
                     {
                         string error = (resulttask.IsCanceled) ? "Cancelled" :
                             ((resulttask.Exception == null) ? "Faulted" : resulttask.Exception.Flatten().InnerException.Message);

                         promise.InvokeAsync("reject", _CefV8Context, CefV8Value.CreateString(error));
                     }

                     _CefV8Context.Exit();
                 });
        }

        private void Execute(CefV8Value[] e, IJSCBridgeCache mapper)
        {
            _JSResultCommand.Execute(GetArguments(mapper, e))
                .ContinueWith(t => SetResult(e, mapper, t));
        }

        public object CValue
        {
            get { return _JSResultCommand; }
        }

        public JSCSGlueType Type
        {
            get { return JSCSGlueType.ResultCommand; }
        }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return Enumerable.Empty<IJSCSGlue>();
        }

        protected override void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            sb.Append("{}");
        }
    }
}
