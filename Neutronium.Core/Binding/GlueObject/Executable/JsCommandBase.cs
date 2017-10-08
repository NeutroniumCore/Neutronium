using System;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    public abstract class JsCommandBase : GlueBase
    {
        private readonly HtmlViewContext _HtmlViewContext;
        private byte _Count = 1;
        private IJavascriptViewModelUpdater ViewModelUpdater => _HtmlViewContext.ViewModelUpdater;

        public uint JsId { get; private set; }
        public virtual IJavascriptObject CachableJsValue => JsValue;
        public JsCsGlueType Type => JsCsGlueType.Command;

        protected readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        protected IWebView WebView => _HtmlViewContext.WebView;
        protected IDispatcher UiDispatcher => _HtmlViewContext.UiDispatcher;
        protected IWebSessionLogger Logger => _HtmlViewContext.Logger;
        protected void SetJsId(uint jsId) => JsId = jsId;

        protected JsCommandBase(HtmlViewContext context, IJavascriptToCSharpConverter converter)
        {
            _JavascriptToCSharpConverter = converter;
            _HtmlViewContext = context;
        }

        public abstract void Execute(IJavascriptObject[] e);
        public abstract void CanExecuteCommand(params IJavascriptObject[] e);
        public abstract void ListenChanges();
        public abstract void UnListenChanges();

        public void UpdateJsObject(IJavascriptObject javascriptObject)
        {
            javascriptObject.Bind("Execute", WebView, Execute);
            javascriptObject.Bind("CanExecute", WebView, CanExecuteCommand);
        }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.AppendCommandDescription();
        }

        public void VisitChildren(Action<IJsCsGlue> visit) { }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
            listener.OnCommand(this);
        }

        protected void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            _Count = (byte) ((_Count == 1) ? 2 : 1);
            UpdateProperty("CanExecuteCount", (f) => f.CreateInt(_Count));
        }

        protected void UpdateCanExecuteValue(bool value)
        {
            UpdateProperty("CanExecuteValue", (f) => f.CreateBool(value));
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
