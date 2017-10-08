using System;
using System.Windows.Input;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    public class JsCommand : JsCommandBase, IJsCsCachableGlue, IExecutableGlue
    {
        public object CValue => _Command;
        private readonly bool _InitialCanExecute = true;
        private readonly ICommand _Command;

        void IJsCsCachableGlue.SetJsId(uint jsId) => base.SetJsId(jsId);

        public JsCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, ICommand command): 
            base(context, converter)
        {
            _Command = command;
            try
            {
                _InitialCanExecute = _Command.CanExecute(null);
            }
            catch { }
        }

        public virtual void SetJsValue(IJavascriptObject value, IJavascriptSessionCache sessionCache)
        {
            SetJsValue(value);
            sessionCache.Cache(this);
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestCommandCreation(_InitialCanExecute);
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
            var parameter = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            UiDispatcher.Dispatch(() => _Command.Execute(parameter));
        }

        public override async void CanExecuteCommand(params IJavascriptObject[] e)
        {
            var parameter = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            var res = await UiDispatcher.EvaluateAsync(() => _Command.CanExecute(parameter));
            UpdateCanExecuteValue(res);
        }
    }
}
