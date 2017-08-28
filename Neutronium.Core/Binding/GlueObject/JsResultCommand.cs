using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neutronium.Core.Extension;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JsResultCommand : GlueBase, IJsCsCachableGlue, IExecutableGlue
    {
        private readonly IResultCommand _JsResultCommand;
        private readonly HtmlViewContext _HtmlViewContext;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;

        public IEnumerable<IJsCsGlue> Children => null;
        public virtual IJavascriptObject CachableJsValue => JsValue;
        public object CValue => _JsResultCommand;
        public JsCsGlueType Type => JsCsGlueType.ResultCommand;
        protected IWebView WebView => _HtmlViewContext.WebView;
        private IDispatcher UiDispatcher => _HtmlViewContext.UiDispatcher;

        private uint _JsId;
        public uint JsId => _JsId;
        void IJsCsCachableGlue.SetJsId(uint jsId) => _JsId = jsId;

        public JsResultCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, IResultCommand resultCommand)
        {
            _HtmlViewContext = context;
            _JavascriptToCSharpConverter = converter;
            _JsResultCommand = resultCommand;
        }

        public void UpdateJsObject(IJavascriptObject javascriptObject)
        {
            IExecutableGlue executable = this;
            javascriptObject.Bind("Execute", WebView, executable.Execute);
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestExecutableCreation();
        }
      
        async void IExecutableGlue.Execute(IJavascriptObject[] e)
        {
            var argument = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            var promise = (e.Length > 1) ? e[1] : null;

            try
            {
                await await UiDispatcher.EvaluateAsync(async () =>
                {
                    var res = await _JsResultCommand.Execute(argument);
                    if (res == null)
                        return;

                    _JavascriptToCSharpConverter.RegisterInSession(res,bridge => SetResult(promise, bridge));
                });
            }
            catch (Exception exception)
            {
                SetError(promise, exception).DoNotWait();
            }
        }

        private async Task SetError(IJavascriptObject promise, Exception exception)
        {
            if (promise == null)
                return;

            await WebView.RunAsync(async () =>
            {
                var errormessage = exception?.Message ?? "Faulted";
                await promise.InvokeAsync("reject", WebView, WebView.Factory.CreateString(errormessage));
            });
        }

        private async void SetResult(IJavascriptObject promise, IJsCsGlue bridgevalue)
        {
            if (promise == null)
                return;

            await WebView.RunAsync(async () =>
            {
                await promise.InvokeAsync("fullfill", WebView, bridgevalue.GetJsSessionValue());
            });
        }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.AppendCommandDescription();
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
        }
    }
}
