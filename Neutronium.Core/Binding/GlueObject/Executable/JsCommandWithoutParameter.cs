using System;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    public class JsCommandWithoutParameter : JsCommandBase, IJsCsCachableGlue, IExecutableGlue
    {
        public object CValue => _Command;
        private readonly ICommandWithoutParameter _Command;
        private bool _CanExecute;

        void IJsCsCachableGlue.SetJsId(uint jsId) => base.SetJsId(jsId);

        public JsCommandWithoutParameter(HtmlViewContext context, IJavascriptToCSharpConverter converter, ICommandWithoutParameter command) :
            base(context, converter)
        {
            _Command = command;
            _CanExecute = _Command.CanExecute;
        }

        public virtual void SetJsValue(IJavascriptObject value, IJavascriptSessionCache sessionCache)
        {
            SetJsValue(value);
            sessionCache.Cache(this);
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestCommandCreation(_CanExecute);
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            visit(this);
        }

        public override void ListenChanges()
        {
            _Command.CanExecuteChanged += _Command_CanExecuteChanged;
        }

        public override void UnListenChanges()
        {
            _Command.CanExecuteChanged -= _Command_CanExecuteChanged;
        }

        public override void Execute(IJavascriptObject[] e)
        {
            UiDispatcher.Dispatch(() => _Command.Execute());
        }

        private void _Command_CanExecuteChanged(object sender, EventArgs e)
        {
            ComputeCanExecute();
        }

        public override void CanExecuteCommand(params IJavascriptObject[] e)
        {
            ComputeCanExecute();
        }

        private async void ComputeCanExecute()
        {
            var res = await UiDispatcher.EvaluateAsync(() => _Command.CanExecute);
            if (res == _CanExecute)
                return;

            _CanExecute = res;
            UpdateCanExecuteValue(res);
        }
    }
}
