using System;
using System.Windows.Input;
using Neutronium.Core.Binding.Converter;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    public class JsCommand : JsCommandBase, IJsCsCachableGlue, IExecutableGlue
    {
        public object CValue => _Command;
        private readonly ICommand _Command;

        void IJsCsCachableGlue.SetJsId(uint jsId) => base.SetJsId(jsId);

        public JsCommand(HtmlViewContext context, IJavascriptToGlueMapper converter, ICommand command): 
            base(context, converter)
        {
            _Command = command;
            try
            {
                _CanExecute = _Command.CanExecute(null);
            }
            catch { }
        }

        public virtual void SetJsValue(IJavascriptObject value, IJavascriptSessionCache sessionCache)
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
            var parameter = JavascriptToGlueMapper.GetFirstArgumentOrNull(e);
            UiDispatcher.Dispatch(() => _Command.Execute(parameter));
        }

        public override async void CanExecuteCommand(params IJavascriptObject[] e)
        {
            var parameter = JavascriptToGlueMapper.GetFirstArgumentOrNull(e);
            await UiDispatcher.RunAsync(() => _CanExecute = _Command.CanExecute(parameter));
            UpdateCanExecuteValue();
        }
    }
}
