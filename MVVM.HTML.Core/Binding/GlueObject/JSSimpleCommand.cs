using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVM.Component;
using MVVM.HTML.Core.Extension;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Binding.GlueObject
{
    public class JSSimpleCommand : GlueBase, IJSObservableBridge
    {
        private readonly ISimpleCommand _JSSimpleCommand;
        private readonly IWebView _WebView;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject JSValue { get; }
        public IJavascriptObject MappedJSValue => _MappedJSValue;
        public object CValue => _JSSimpleCommand;
        public JSCSGlueType Type => JSCSGlueType.SimpleCommand;

        public JSSimpleCommand(IWebView webView, IJavascriptToCSharpConverter converter, ISimpleCommand simpleCommand)
        {
            _WebView = webView;
            _JavascriptToCSharpConverter = converter;
            _JSSimpleCommand = simpleCommand;
            JSValue = _WebView.Factory.CreateObject(true);
        }

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", _WebView, Execute);
        }

        private void Execute(IJavascriptObject[] e)
        {
            _JSSimpleCommand.Execute(_JavascriptToCSharpConverter.GetFirstArgumentOrNull(e));
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
