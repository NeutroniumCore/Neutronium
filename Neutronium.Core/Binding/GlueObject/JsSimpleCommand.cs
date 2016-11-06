using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsSimpleCommand : GlueBase, IJSObservableBridge
    {
        private readonly ISimpleCommand _JSSimpleCommand;
        private readonly IWebView _WebView;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject JSValue { get; private set; }
        public IJavascriptObject MappedJSValue => _MappedJSValue;
        public object CValue => _JSSimpleCommand;
        public JsCsGlueType Type => JsCsGlueType.SimpleCommand;

        public JsSimpleCommand(IWebView webView, IJavascriptToCSharpConverter converter, ISimpleCommand simpleCommand)
        {
            _WebView = webView;
            _JavascriptToCSharpConverter = converter;
            _JSSimpleCommand = simpleCommand;
        }

        protected override bool LocalComputeJavascriptValue(IJavascriptObjectFactory factory)
        {
            if (JSValue != null)
                return false;

            JSValue = factory.CreateObject(true);
            return true;
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

        public override IEnumerable<IJSCSGlue> GetChildren()
        {
            return Enumerable.Empty<IJSCSGlue>();
        }

        protected override void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            sb.Append("{}");
        }
    }
}
