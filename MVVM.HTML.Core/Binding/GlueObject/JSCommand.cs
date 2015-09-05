using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading;
using System.Windows;

using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Window;



namespace MVVM.HTML.Core.HTMLBinding
{
    public class JSCommand : GlueBase, IJSObservableBridge
    {
        private int _Count = 1;
        private IWebView _IWebView;
        private IDispatcher _UIDispatcher;

        public JSCommand(IWebView iCefV8Context, IDispatcher iUIDispatcher, ICommand icValue)
        {
            _UIDispatcher = iUIDispatcher;
            _IWebView = iCefV8Context;
            _Command = icValue;
       
            bool canexecute = true;
            try
            {
                canexecute = _Command.CanExecute(null);
            }
            catch { }

            JSValue = _IWebView.Evaluate(() =>
                {
                    IJavascriptObject res = _IWebView.Factory.CreateObject(true);
                    res.SetValue("CanExecuteValue", _IWebView.Factory.CreateBool(canexecute));
                    res.SetValue("CanExecuteCount", _IWebView.Factory.CreateInt(_Count)); 
                    return res;       
                });

        }

        public void ListenChanges()
        {
            _Command.CanExecuteChanged += _Command_CanExecuteChanged;
        }

        public void UnListenChanges()
        {
            _Command.CanExecuteChanged -= _Command_CanExecuteChanged;
        }

        private void _Command_CanExecuteChanged(object sender, EventArgs e)
        {
            _Count = (_Count == 1) ? 2 : 1;
            this._IWebView.RunAsync(() =>
            {
                _MappedJSValue.Invoke("CanExecuteCount", _IWebView, _IWebView.Factory.CreateInt(_Count));
            });
        }


        public IJavascriptObject JSValue { get; private set; }

        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(IJavascriptObject ijsobject, IJSCBridgeCache mapper)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", _IWebView, (c, o, e) => ExecuteCommand(e, mapper));
            _MappedJSValue.Bind("CanExecute", _IWebView, (c, o, e) => CanExecuteCommand(e, mapper));
        }

        private object Convert(IJSCBridgeCache mapper, IJavascriptObject value)
        {
            var found = mapper.GetCachedOrCreateBasic(value,null);
            return (found != null) ? found.CValue : null;
        }

        private object GetArguments(IJSCBridgeCache mapper, IJavascriptObject[] e)
        {
            return (e.Length == 0) ? null : Convert(mapper, e[0]);
        }

        private void ExecuteCommand(IJavascriptObject[] e, IJSCBridgeCache mapper)
        {
            _UIDispatcher.RunAsync(() => _Command.Execute(GetArguments(mapper, e)));
            //CefCoreSessionSingleton.Session.UIDispatcher.RunAsync(() => _Command.Execute(GetArguments(mapper, e)));
        }

        private void CanExecuteCommand(IJavascriptObject[] e, IJSCBridgeCache mapper)
        {
            bool res = _Command.CanExecute(GetArguments(mapper, e));
            _MappedJSValue.Invoke("CanExecuteValue", _IWebView, _IWebView.Factory.CreateBool(res));
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
