using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading;

using Xilium.CefGlue;

using MVVM.CEFGlue.CefGlueHelper;
using System.Windows;
using MVVM.CEFGlue.Infra;
using MVVM.CEFGlue.CefSession;



namespace MVVM.CEFGlue.HTMLBinding
{
    public class JSCommand : GlueBase, IJSObservableBridge
    {
        private int _Count = 1;
        private CefV8CompleteContext _CefV8Context;
        public JSCommand(CefV8CompleteContext iCefV8Context, IJSOBuilder builder, ICommand icValue)
        {
            _CefV8Context = iCefV8Context;
            _Command = icValue;
       
            bool canexecute = true;
            try
            {
                canexecute = _Command.CanExecute(null);
            }
            catch { }

            JSValue = _CefV8Context.Evaluate(() =>
                {
                    //_CefV8Context.Enter();
                    CefV8Value res = builder.CreateJSO();
                    res.SetValue("CanExecuteValue", CefV8Value.CreateBool(canexecute),CefV8PropertyAttribute.None);
                    res.SetValue("CanExecuteCount", CefV8Value.CreateInt(_Count), CefV8PropertyAttribute.None); 
                    //_CefV8Context.Exit();
                    return res;       
                });
            //.Result;

        }

        public void ListenChanges()
        {
            _Command.CanExecuteChanged += _Command_CanExecuteChanged;
        }

        public void UnListenChanges()
        {
            _Command.CanExecuteChanged -= _Command_CanExecuteChanged;
        }

        private async void _Command_CanExecuteChanged(object sender, EventArgs e)
        {
            _Count = (_Count == 1) ? 2 : 1;
            this._CefV8Context.RunAsync(() =>
            {
                //_CefV8Context.Enter();
                _MappedJSValue.Invoke("CanExecuteCount", _CefV8Context,CefV8Value.CreateInt(_Count));
                //_CefV8Context.Exit();
            });
            //WebCore.QueueWork(() =>
            //        ((JSObject)_MappedJSValue).InvokeAsync("CanExecuteCount", new JSValue(_Count))
            //);
        }


        public CefV8Value JSValue { get; private set; }

        private CefV8Value _MappedJSValue;

        public CefV8Value MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(CefV8Value ijsobject, IJSCBridgeCache mapper)
        {
            _MappedJSValue = ijsobject;
            CefV8Value mapped = ((CefV8Value)_MappedJSValue);
            mapped.Bind("Execute", _CefV8Context,(c, o, e) => ExecuteCommand(e, mapper));
            mapped.Bind("CanExecute",_CefV8Context, (c, o, e) => CanExecuteCommand(e, mapper));
        }

        private object Convert(IJSCBridgeCache mapper, CefV8Value value)
        {
            var found = mapper.GetCachedOrCreateBasic(value,null);
            return (found != null) ? found.CValue : null;
        }

        private object GetArguments(IJSCBridgeCache mapper, CefV8Value[] e)
        {
            return (e.Length == 0) ? null : Convert(mapper, e[0]);
        }

        private void ExecuteCommand(CefV8Value[] e, IJSCBridgeCache mapper)
        {
            CefCoreSessionSingleton.Session.Dispatcher.RunAsync(() => _Command.Execute(GetArguments(mapper, e)));
        }

        private void CanExecuteCommand(CefV8Value[] e, IJSCBridgeCache mapper)
        {
            bool res = _Command.CanExecute(GetArguments(mapper, e));
            _MappedJSValue.Invoke("CanExecuteValue", _CefV8Context, CefV8Value.CreateBool(res));
        }

        private ICommand _Command;
        public object CValue { get { return _Command; } }

        public JSCSGlueType Type { get { return JSCSGlueType.Command; } }

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
