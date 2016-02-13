using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Binding.Extension;
using MVVM.HTML.Core.Window;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class JSCommand : GlueBase, IJSObservableBridge
    {
        private readonly IWebView _WebView;
        private readonly IDispatcher _UIDispatcher;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly ICommand _Command;
        private IJavascriptObject _MappedJSValue;
        private int _Count = 1;

        public IJavascriptObject JSValue { get; private set; }
        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }
        public object CValue { get { return _Command; } }
        public JSCSGlueType Type { get { return JSCSGlueType.Command; } }

        public JSCommand(IWebView webView, IJavascriptToCSharpConverter converter, IDispatcher uiDispatcher, ICommand command)
        {
            _UIDispatcher = uiDispatcher;
            _JavascriptToCSharpConverter = converter;
            _WebView = webView;
            _Command = command;
       
            bool canexecute = true;
            try
            {
                canexecute = _Command.CanExecute(null);
            }
            catch { }

            JSValue = _WebView.Evaluate(() =>
                {
                    var res = _WebView.Factory.CreateObject(true);
                    res.SetValue("CanExecuteValue", _WebView.Factory.CreateBool(canexecute));
                    res.SetValue("CanExecuteCount", _WebView.Factory.CreateInt(_Count)); 
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
#region Knockout
            _WebView.RunAsync(() =>
            {
                _MappedJSValue.Invoke("CanExecuteCount", _WebView, _WebView.Factory.CreateInt(_Count));
            });
#endregion
        }

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", _WebView, ExecuteCommand);
            _MappedJSValue.Bind("CanExecute", _WebView, CanExecuteCommand);
        }

        private void ExecuteCommand(IJavascriptObject[] e)
        {
            _UIDispatcher.RunAsync(() => _Command.Execute(_JavascriptToCSharpConverter.GetFirstArgumentOrNull(e)));
        }

        private void CanExecuteCommand(IJavascriptObject[] e)
        {
            bool res = _Command.CanExecute(_JavascriptToCSharpConverter.GetFirstArgumentOrNull(e));
#region Knockout
            _MappedJSValue.Invoke("CanExecuteValue", _WebView, _WebView.Factory.CreateBool(res));
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
