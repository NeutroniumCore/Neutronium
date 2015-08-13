using MVVM.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xilium.CefGlue;

using MVVM.CEFGlue.CefGlueHelper;
using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;

namespace MVVM.CEFGlue.HTMLBinding
{
    public class JSSimpleCommand : GlueBase, IJSObservableBridge
    {
        private ISimpleCommand _JSSimpleCommand;
        private IWebView _CefV8Context;
        public JSSimpleCommand(IWebView iCefV8Context, IJSOBuilder builder, ISimpleCommand icValue)
        {
            _CefV8Context = iCefV8Context;
            _JSSimpleCommand = icValue;
            JSValue = builder.CreateJSO();    
        }

        public CefV8Value JSValue { get; private set; }

        private CefV8Value _MappedJSValue;

        public CefV8Value MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(CefV8Value ijsobject, IJSCBridgeCache mapper)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", _CefV8Context, (c, o, e) => Execute(e, mapper));
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

        private void Execute(CefV8Value[] e, IJSCBridgeCache mapper)
        {
            _JSSimpleCommand.Execute(GetArguments(mapper, e));
        }

        public object CValue
        {
            get { return _JSSimpleCommand; }
        }

        public JSCSGlueType Type
        {
            get { return JSCSGlueType.SimpleCommand; }
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
