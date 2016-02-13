using System.Collections.Generic;
using System.Linq;
using System.Text;

using MVVM.Component;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Binding.Extension;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class JSSimpleCommand : GlueBase, IJSObservableBridge
    {
        private readonly ISimpleCommand _JSSimpleCommand;
        private readonly IWebView _WebView;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject JSValue { get; private set; }
        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }
        public object CValue { get { return _JSSimpleCommand; } }
        public JSCSGlueType Type { get { return JSCSGlueType.SimpleCommand; } }

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
