using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsSimpleCommand : GlueBase, IJSObservableBridge
    {
        private readonly ISimpleCommand _JSSimpleCommand;
        private readonly HTMLViewContext _HTMLViewContext;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject JSValue { get; private set; }
        public IJavascriptObject MappedJSValue => _MappedJSValue;
        public object CValue => _JSSimpleCommand;
        public JsCsGlueType Type => JsCsGlueType.SimpleCommand;
        private IWebView WebView => _HTMLViewContext.WebView;
        private IDispatcher UIDispatcher => _HTMLViewContext.UIDispatcher;

        public JsSimpleCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, ISimpleCommand simpleCommand)
        {
            _HTMLViewContext = context;
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
            _MappedJSValue.Bind("Execute", WebView, Execute);
        }

        private void Execute(IJavascriptObject[] e)
        {
            var parameter = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            UIDispatcher.RunAsync(() => _JSSimpleCommand.Execute(parameter));
        }

        public override IEnumerable<IJSCSGlue> GetChildren()
        {
            return Enumerable.Empty<IJSCSGlue>();
        }

        protected override void ComputeString(NameContext context)
        {
            context.Append("{}");
        }
    }
}
