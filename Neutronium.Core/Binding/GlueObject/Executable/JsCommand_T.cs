using System;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    public class JsCommand<T> : JsCommandBase, IJsCsCachableGlue, IExecutableGlue
    {
        public object CValue => _Command;
        private readonly ICommand<T> _Command;

        void IJsCsCachableGlue.SetJsId(uint jsId) => base.SetJsId(jsId);

        public JsCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, ICommand<T> command) :
            base(context, converter)
        {
            _Command = command;
        }

        public virtual void SetJsValue(IJavascriptObject value, IJavascriptSessionCache sessionCache)
        {
            SetJsValue(value);
            sessionCache.Cache(this);
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestCommandCreation(true);
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            visit(this);
        }

        public override void ListenChanges()
        {
            _Command.CanExecuteChanged += Command_CanExecuteChanged;
        }

        public override void UnListenChanges()
        {
            _Command.CanExecuteChanged -= Command_CanExecuteChanged;
        }

        public override void Execute(IJavascriptObject[] e)
        {
            var parameter = _JavascriptToCSharpConverter.GetFirstArgument<T>(e);
            if (!parameter.Success)
            {
                Logger.Error($"Impossible to call Execute on command<{typeof(T)}>, no matching argument found, received:{parameter.TentativeValue} of type:{parameter.TentativeValue?.GetType()} expectedType: {typeof(T)}");
                return;
            }

            UiDispatcher.Dispatch(() => _Command.Execute(parameter.Value));
        }

        internal override async void CanExecuteCommand(params IJavascriptObject[] e)
        {
            var parameter = _JavascriptToCSharpConverter.GetFirstArgument<T>(e);
            if (!parameter.Success)
            {
                Logger.Error($"Impossible to call CanExecuteCommand on command<{typeof(T)}>, no matching argument found, received:{parameter.TentativeValue} of type:{parameter.TentativeValue?.GetType()} expectedType: {typeof(T)}");
                return;
            }
            var res = await UiDispatcher.EvaluateAsync(() => _Command.CanExecute(parameter.Value));
            UpdateCanExecuteValue(res);
        }
    }
}
