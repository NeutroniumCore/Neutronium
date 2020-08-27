using System;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    internal class JsCommand<T> : JsCommandBase, IJsCsCachableGlue, IExecutableGlue
    {
        public object CValue => _Command;
        private readonly ICommand<T> _Command;

        void IJsCsCachableGlue.SetJsId(uint jsId) => base.SetJsId(jsId);

        internal JsCommand(HtmlViewContext context, IJavascriptToGlueMapper converter, ICommand<T> command) :
            base(context, converter)
        {
            _Command = command;
            _CanExecute = true;
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
            var parameter = JavascriptToGlueMapper.GetFirstArgument<T>(e);
            if (!parameter.Success)
            {
                Logger.Error($"Impossible to call Execute on command<{typeof(T)}>, no matching argument found, received:{parameter.TentativeValue} of type:{parameter.TentativeValue?.GetType()} expectedType: {typeof(T)}");
                return;
            }

            UiDispatcher.Dispatch(() => _Command.Execute(parameter.Value));
        }

        public override async void CanExecuteCommand(params IJavascriptObject[] e)
        {
            var parameter = JavascriptToGlueMapper.GetFirstArgument<T>(e);
            if (!parameter.Success)
            {
                Logger.Error($"Impossible to call CanExecuteCommand on command<{typeof(T)}>, no matching argument found, received:{parameter.TentativeValue} of type:{parameter.TentativeValue?.GetType()} expectedType: {typeof(T)}");
                return;
            }
            await UiDispatcher.RunAsync(() => _CanExecute = _Command.CanExecute(parameter.Value));
            UpdateCanExecuteValue();
        }
    }
}
