using System.Collections.Generic;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsSimpleCommand : GlueBase, IJSCSCachableGlue, IExecutableGlue
    {
        private readonly ISimpleCommand _JSSimpleCommand;
        private readonly HTMLViewContext _HTMLViewContext;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;

        public virtual IJavascriptObject CachableJSValue => JSValue;
        public object CValue => _JSSimpleCommand;
        public JsCsGlueType Type => JsCsGlueType.SimpleCommand;
        protected IWebView WebView => _HTMLViewContext.WebView;
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

        public void UpdateJsObject(IJavascriptObject javascriptObject)
        {
            IExecutableGlue executable = this;
            javascriptObject.Bind("Execute", WebView, executable.Execute);
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestExecutableCreation();
        }
    
        void IExecutableGlue.Execute(IJavascriptObject[] e)
        {
            var parameter = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            UIDispatcher.RunAsync(() => _JSSimpleCommand.Execute(parameter));
        }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return null;
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
