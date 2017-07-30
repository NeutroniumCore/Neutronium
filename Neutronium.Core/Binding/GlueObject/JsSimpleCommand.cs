using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsSimpleCommand : GlueBase, IJSCSMappedBridge
    {
        private readonly ISimpleCommand _JSSimpleCommand;
        private readonly HTMLViewContext _HTMLViewContext;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject CachableJSValue => _MappedJSValue;
        public object CValue => _JSSimpleCommand;
        public JsCsGlueType Type => JsCsGlueType.SimpleCommand;
        private IWebView WebView => _HTMLViewContext.WebView;
        private IDispatcher UIDispatcher => _HTMLViewContext.UIDispatcher;

        private uint _JsId;
        public uint JsId => _JsId;
        void IJSCSCachableGlue.SetJsId(uint jsId) => _JsId = jsId;

        public JsSimpleCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, ISimpleCommand simpleCommand)
        {
            _HTMLViewContext = context;
            _JavascriptToCSharpConverter = converter;
            _JSSimpleCommand = simpleCommand;
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestObjectCreation();
        }

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {
            _MappedJSValue = jsobject;
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

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.AppendCommandDescription();
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
        }
    }
}
