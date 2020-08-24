using System;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    internal abstract class JsCommandBase : GlueBase
    {
        private readonly HtmlViewContext _HtmlViewContext;
        private byte _Count = 1;
        private IJavascriptViewModelUpdater ViewModelUpdater => _HtmlViewContext.ViewModelUpdater;

        public uint JsId { get; private set; }
        public virtual IJavascriptObject CachableJsValue => JsValue;
        public JsCsGlueType Type => JsCsGlueType.Command;

        protected internal readonly IJavascriptToGlueMapper JavascriptToGlueMapper;
        protected IWebView WebView => _HtmlViewContext.WebView;
        protected IDispatcher UiDispatcher => _HtmlViewContext.UiDispatcher;
        protected IWebSessionLogger Logger => _HtmlViewContext.Logger;
        protected void SetJsId(uint jsId) => JsId = jsId;
        protected bool _CanExecute;

        internal byte NextUpdateCount => (byte)((_Count == 1) ? 2 : 1);
        internal byte CurrentUpdateCount => _Count;

        protected internal JsCommandBase(HtmlViewContext context, IJavascriptToGlueMapper converter)
        {
            JavascriptToGlueMapper = converter;
            _HtmlViewContext = context;
        }

        public abstract void Execute(IJavascriptObject[] e);
        public abstract void CanExecuteCommand(params IJavascriptObject[] e);

        public void UpdateJsObject(IJavascriptObject javascriptObject)
        {
            javascriptObject.Bind("Execute", WebView, Execute);
            javascriptObject.Bind("CanExecute", WebView, CanExecuteCommand);
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestCommandCreation(_CanExecute);
        }

        protected override void ComputeString(IDescriptionBuilder context)
        {
            context.AppendCommandDescription(_CanExecute);
        }

        public void VisitChildren(Action<IJsCsGlue> visit) { }

        internal void UpdateCount(byte updateCount)
        {
            _Count = updateCount;
        }

        internal void SetUpdateCountOnJsContext()
        {
            var newValue = WebView.Factory.CreateInt(_Count);
            ViewModelUpdater.UpdateProperty(CachableJsValue, "CanExecuteCount", newValue, false);
        }

        protected void UpdateCanExecuteValue()
        {
            UpdateProperty("CanExecuteValue", (f) => f.CreateBool(_CanExecute));
        }

        private void UpdateProperty(string propertyName, Func<IJavascriptObjectFactory, IJavascriptObject> factory)
        {
            WebView?.Dispatch(() =>
            {
                var newValue = factory(WebView.Factory);
                ViewModelUpdater.UpdateProperty(CachableJsValue, propertyName, newValue, false);
            });
        }
    }
}
