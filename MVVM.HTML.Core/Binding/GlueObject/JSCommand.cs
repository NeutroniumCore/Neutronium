using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Window;
using MVVM.HTML.Core.Binding.Mapping;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class JSCommand : GlueBase, IJSObservableBridge
    {
        private readonly IWebView _IWebView;
        private readonly IDispatcher _UIDispatcher;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly ICommand _Command;
        private IJavascriptObject _MappedJSValue;
        private int _Count = 1;

        public IJavascriptObject JSValue { get; private set; }
        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }
        public object CValue { get { return _Command; } }
        public JSCSGlueType Type { get { return JSCSGlueType.Command; } }

        public JSCommand(IWebView iCefV8Context, IJavascriptToCSharpConverter converter, IDispatcher iUIDispatcher, ICommand icValue)
        {
            _UIDispatcher = iUIDispatcher;
            _JavascriptToCSharpConverter = converter;
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
                    var res = _IWebView.Factory.CreateObject(true);
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
            _IWebView.RunAsync(() =>
            {
                _MappedJSValue.Invoke("CanExecuteCount", _IWebView, _IWebView.Factory.CreateInt(_Count));
            });
        }

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", _IWebView, (c, o, e) => ExecuteCommand(e));
            _MappedJSValue.Bind("CanExecute", _IWebView, (c, o, e) => CanExecuteCommand(e));
        }

        private void ExecuteCommand(IJavascriptObject[] e)
        {
            _UIDispatcher.RunAsync(() => _Command.Execute(_JavascriptToCSharpConverter.GetArguments(e)));
        }

        private void CanExecuteCommand(IJavascriptObject[] e)
        {
            bool res = _Command.CanExecute(_JavascriptToCSharpConverter.GetArguments(e));
#region Knockout
            _MappedJSValue.Invoke("CanExecuteValue", _IWebView, _IWebView.Factory.CreateBool(res));
#endregion
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
