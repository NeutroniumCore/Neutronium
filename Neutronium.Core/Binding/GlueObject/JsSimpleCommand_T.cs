using System;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JsSimpleCommand<T> : JsSimpleCommandBase, IJsCsCachableGlue, IExecutableGlue
    {
        private readonly ISimpleCommand<T> _JsSimpleCommand;
        public object CValue => _JsSimpleCommand;

        void IJsCsCachableGlue.SetJsId(uint jsId) => SetJsId(jsId);

        public JsSimpleCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, ISimpleCommand<T> simpleCommand):
            base(context, converter)
        {
            _JsSimpleCommand = simpleCommand;
        }

        public virtual void SetJsValue(IJavascriptObject value, IJavascriptSessionCache sessionCache)
        {
            SetJsValue(value);
            sessionCache.Cache(this);
        }

        public void UpdateJsObject(IJavascriptObject javascriptObject)
        {
            IExecutableGlue executable = this;
            javascriptObject.Bind("Execute", WebView, executable.Execute);
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            visit(this);
        }

        void IExecutableGlue.Execute(IJavascriptObject[] e)
        {
            var parameter = _JavascriptToCSharpConverter.GetFirstArgumentOrNull<T>(e);
            if (!parameter.Success) 
            {
                Logger.Error("Impossible to call simple command, no matching argument found");
                return;
            }

            UiDispatcher.Dispatch(() => _JsSimpleCommand.Execute(parameter.Value));
        }
    }
}
