using System;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueObject.Executable 
{
    public class JsSimpleCommandBase : GlueBase
    {
        private readonly HtmlViewContext _HtmlViewContext;
        protected readonly IJavascriptToGlueMapper JavascriptToGlueMapper;

        public virtual IJavascriptObject CachableJsValue => JsValue;

        public JsCsGlueType Type => JsCsGlueType.SimpleCommand;
        protected IWebView WebView => _HtmlViewContext.WebView;
        protected IDispatcher UiDispatcher => _HtmlViewContext.UiDispatcher;
        protected IWebSessionLogger Logger => _HtmlViewContext.Logger;

        public uint JsId { get; private set; }

        protected void SetJsId(uint jsId) => JsId = jsId;

        public JsSimpleCommandBase(HtmlViewContext context, IJavascriptToGlueMapper converter)
        {
            _HtmlViewContext = context;
            JavascriptToGlueMapper = converter;
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestExecutableCreation();
        }

        public void VisitChildren(Action<IJsCsGlue> visit) { }

        protected override void ComputeString(IDescriptionBuilder context)
        {
            context.AppendCommandDescription(true);
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
        }
    }
}
