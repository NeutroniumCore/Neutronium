using System.Collections.Generic;
using System.Linq;
using System.Text;

using MVVM.Component;

using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Binding.Mapping;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class JSSimpleCommand : GlueBase, IJSObservableBridge
    {
        private readonly ISimpleCommand _JSSimpleCommand;
        private readonly IWebView _IWebView;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        public JSSimpleCommand(IWebView iCefV8Context, IJavascriptToCSharpConverter converter, ISimpleCommand icValue)
        {
            _IWebView = iCefV8Context;
            _JavascriptToCSharpConverter = converter;
            _JSSimpleCommand = icValue;
            JSValue = _IWebView.Factory.CreateObject(true);
        }

        public IJavascriptObject JSValue { get; private set; }

        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", _IWebView, (c, o, e) => Execute(e));
        }

        private object Convert(IJavascriptObject value)
        {
            var found = _JavascriptToCSharpConverter.GetCachedOrCreateBasic(value, null);
            return (found != null) ? found.CValue : null;
        }

        private object GetArguments(IJavascriptObject[] e)
        {
            return (e.Length == 0) ? null : Convert(e[0]);
        }

        private void Execute(IJavascriptObject[] e)
        {
            _JSSimpleCommand.Execute(GetArguments(e));
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
