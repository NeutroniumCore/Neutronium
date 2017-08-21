using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Neutronium.Core.Extension;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JSCommand : GlueBase, IJSCSCachableGlue, IExecutableGlue
    {
        private readonly HTMLViewContext _HTMLViewContext;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly ICommand _Command;
        private int _Count = 1;

        public virtual IJavascriptObject CachableJSValue => JSValue;
        public object CValue => _Command;
        public JsCsGlueType Type => JsCsGlueType.Command;
        protected IWebView WebView => _HTMLViewContext.WebView;
        private IDispatcher UIDispatcher => _HTMLViewContext.UIDispatcher;
        private IJavascriptViewModelUpdater ViewModelUpdater => _HTMLViewContext.ViewModelUpdater;

        private uint _JsId;
        public uint JsId => _JsId;
        void IJSCSCachableGlue.SetJsId(uint jsId) => _JsId = jsId;

        private bool _InitialCanExecute = true;

        public JSCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, ICommand command)
        {
            _JavascriptToCSharpConverter = converter;
            _HTMLViewContext = context;
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
            UIDispatcher.RunAsync(() => _Command.Execute(parameter));
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
            var res = await UIDispatcher.EvaluateAsync(() => _Command.CanExecute(parameter));
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
            ViewModelUpdater.UpdateProperty(CachableJSValue, propertyName, newValue, false);
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
            listener.OnCommand(this);
        }
    }
}
