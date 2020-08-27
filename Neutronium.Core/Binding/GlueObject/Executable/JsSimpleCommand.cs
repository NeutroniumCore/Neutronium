using System;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable 
{
    internal class JsSimpleCommand : JsSimpleCommandBase, IJsCsCachableGlue, IExecutableGlue 
    {
        private readonly ISimpleCommand _JsSimpleCommand;
        public object CValue => _JsSimpleCommand;

        public JsSimpleCommand(HtmlViewContext context, IJavascriptToGlueMapper converter, ISimpleCommand simpleCommand):
            base(context, converter)
        {
            _JsSimpleCommand = simpleCommand;
        }

        void IJsCsCachableGlue.SetJsId(uint jsId) => SetJsId(jsId);

        void IExecutableGlue.Execute(IJavascriptObject[] e)
        {
            UiDispatcher.Dispatch(() => _JsSimpleCommand.Execute());
        }

        public virtual void SetJsValue(IJavascriptObject value, ISessionCache sessionCache) 
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
