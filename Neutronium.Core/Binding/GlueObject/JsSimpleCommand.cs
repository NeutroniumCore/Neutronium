using System;
using System.Collections.Generic;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsSimpleCommand : GlueBase, IJsCsCachableGlue, IExecutableGlue
    {
        private readonly ISimpleCommand _JsSimpleCommand;
        private readonly HtmlViewContext _HtmlViewContext;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;

        public virtual IJavascriptObject CachableJsValue => JsValue;
        public object CValue => _JsSimpleCommand;
        public IEnumerable<IJsCsGlue> Children => null;

        public JsCsGlueType Type => JsCsGlueType.SimpleCommand;
        protected IWebView WebView => _HtmlViewContext.WebView;
        private IDispatcher UiDispatcher => _HtmlViewContext.UiDispatcher;

        public uint JsId { get; private set; }

        void IJsCsCachableGlue.SetJsId(uint jsId) => JsId = jsId;

        public JsSimpleCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, ISimpleCommand simpleCommand)
        {
            _HtmlViewContext = context;
            _JavascriptToCSharpConverter = converter;
            _JsSimpleCommand = simpleCommand;
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

        public void VisitChildren(Func<IJsCsGlue, bool> visit)
        {
            visit(this);
        }

        void IExecutableGlue.Execute(IJavascriptObject[] e)
        {
            var parameter = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            UiDispatcher.Dispatch(() => _JsSimpleCommand.Execute(parameter));
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
