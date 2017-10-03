using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable 
{
    internal class JsSimpleCommand : JsSimpleCommandBase, IJsCsCachableGlue, IExecutableGlue 
    {
        private ISimpleCommand _JsSimpleCommand;
        public object CValue => _JsSimpleCommand;

        public JsSimpleCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, ISimpleCommand simpleCommand):
            base(context, converter)
        {
            _JsSimpleCommand = simpleCommand;
        }

        void IJsCsCachableGlue.SetJsId(uint jsId) => SetJsId(jsId);

        void IExecutableGlue.Execute(IJavascriptObject[] e)
        {
            UiDispatcher.Dispatch(() => _JsSimpleCommand.Execute());
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
    }
}
