using System;
using System.Collections.Generic;
using System.Windows.Input;
using Neutronium.Core.Extension;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsCommand : GlueBase, IJsCsCachableGlue, IExecutableGlue
    {
        private readonly HtmlViewContext _HtmlViewContext;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly ICommand _Command;
        private int _Count = 1;

        public virtual IJavascriptObject CachableJsValue => JsValue;
        public object CValue => _Command;
        public JsCsGlueType Type => JsCsGlueType.Command;
        public IEnumerable<IJsCsGlue> Children => null;

        protected IWebView WebView => _HtmlViewContext.WebView;
        private IDispatcher UiDispatcher => _HtmlViewContext.UiDispatcher;
        private IJavascriptViewModelUpdater ViewModelUpdater => _HtmlViewContext.ViewModelUpdater;

        private uint _JsId;
        public uint JsId => _JsId;
        void IJsCsCachableGlue.SetJsId(uint jsId) => _JsId = jsId;

        private readonly bool _InitialCanExecute = true;

        public JsCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, ICommand command)
        {
            _JavascriptToCSharpConverter = converter;
            _HtmlViewContext = context;
            _Command = command;

            try
            {
                _InitialCanExecute = _Command.CanExecute(null);
            }
            catch { }
        }

        public void UpdateJsObject(IJavascriptObject javascriptObject)
        {
            javascriptObject.Bind("Execute", WebView, Execute);
            javascriptObject.Bind("CanExecute", WebView, CanExecuteCommand);
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestCommandCreation(_InitialCanExecute);
        }

        public void VisitChildren(Func<IJsCsGlue, bool> visit)
        {
            visit(this);
        }

        public void ListenChanges()
        {
            _Command.CanExecuteChanged += Command_CanExecuteChanged;
        }

        public void UnListenChanges()
        {
            _Command.CanExecuteChanged -= Command_CanExecuteChanged;
        }

        public void Execute(IJavascriptObject[] e)
        {
            var parameter = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            UiDispatcher.RunAsync(() => _Command.Execute(parameter));
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            _Count = (_Count == 1) ? 2 : 1;
            WebView?.RunAsync(() =>
            {
                UpdateProperty("CanExecuteCount", (f) => f.CreateInt(_Count));
            });
        }

        internal async void CanExecuteCommand(params IJavascriptObject[] e)
        {
            var parameter = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            var res = await UiDispatcher.EvaluateAsync(() => _Command.CanExecute(parameter));
            if (WebView == null)
                return;
            await WebView.RunAsync(() =>
            {
                UpdateProperty("CanExecuteValue", (f) => f.CreateBool(res));
            });
        }

        private void UpdateProperty(string propertyName, Func<IJavascriptObjectFactory, IJavascriptObject> factory)
        {
            var newValue = factory(WebView.Factory);
            ViewModelUpdater.UpdateProperty(CachableJsValue, propertyName, newValue, false);
        }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.AppendCommandDescription();
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
            listener.OnCommand(this);
        }
    }
}
