using System;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    internal class JsCommandWithoutParameter : JsCommandBase, IJsCsCachableGlue, IExecutableGlue
    {
        public object CValue => _Command;
        private readonly ICommandWithoutParameter _Command;

        void IJsCsCachableGlue.SetJsId(uint jsId) => base.SetJsId(jsId);

        internal JsCommandWithoutParameter(HtmlViewContext context, IJavascriptToGlueMapper converter, ICommandWithoutParameter command) :
            base(context, converter)
        {
            _Command = command;
            _CanExecute = _Command.CanExecute;
        }

        public virtual void SetJsValue(IJavascriptObject value, ISessionCache sessionCache)
        {
            SetJsValue(value);
            sessionCache.Cache(this);
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            visit(this);
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
            listener.OnCommand(_Command);
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
            await UiDispatcher.RunAsync(() => _CanExecute = _Command.CanExecute);
            UpdateCanExecuteValue();
        }
    }
}
