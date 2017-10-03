using System;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueObject.Executable 
{
    internal class JsSimpleCommandBase : GlueBase
    {
        private readonly HtmlViewContext _HtmlViewContext;
        protected readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;

        public virtual IJavascriptObject CachableJsValue => JsValue;

        public JsCsGlueType Type => JsCsGlueType.SimpleCommand;
        protected IWebView WebView => _HtmlViewContext.WebView;
        protected IDispatcher UiDispatcher => _HtmlViewContext.UiDispatcher;
        protected IWebSessionLogger Logger => _HtmlViewContext.Logger;

        public uint JsId { get; private set; }

        protected void SetJsId(uint jsId) => JsId = jsId;

        public JsSimpleCommandBase(HtmlViewContext context, IJavascriptToCSharpConverter converter)
        {
            _HtmlViewContext = context;
            _JavascriptToCSharpConverter = converter;
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestExecutableCreation();
        }

        public void VisitChildren(Action<IJsCsGlue> visit) { }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.AppendCommandDescription();
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
        }
    }
}
