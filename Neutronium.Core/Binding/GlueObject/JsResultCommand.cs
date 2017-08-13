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
    internal class JsResultCommand : GlueBase, IJSCSCachableGlue, IExecutableGlue
    {
        private readonly IResultCommand _JSResultCommand;
        private readonly HTMLViewContext _HTMLViewContext;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;

        public virtual IJavascriptObject CachableJSValue => JSValue;
        public object CValue => _JSResultCommand;
        public JsCsGlueType Type => JsCsGlueType.ResultCommand;
        protected IWebView WebView => _HTMLViewContext.WebView;
        private IDispatcher UIDispatcher => _HTMLViewContext.UIDispatcher;

        private uint _JsId;
        public uint JsId => _JsId;
        void IJSCSCachableGlue.SetJsId(uint jsId) => _JsId = jsId;

        public JsResultCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, IResultCommand resultCommand)
        {
            _HTMLViewContext = context;
            _JavascriptToCSharpConverter = converter;
            _JSResultCommand = resultCommand;
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
                await await UIDispatcher.EvaluateAsync(async () =>
                {
                    var res = await _JSResultCommand.Execute(argument);
                    if (res == null)
                        return;

                    await _JavascriptToCSharpConverter.RegisterInSession(res,bridge => SetResult(promise, bridge));
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

        private async void SetResult(IJavascriptObject promise, IJSCSGlue bridgevalue)
        {
            if (promise == null)
                return;

            await WebView.RunAsync(async () =>
            {
                await promise.InvokeAsync("fullfill", WebView, bridgevalue.GetJSSessionValue());
            });
        }

        public override IEnumerable<IJSCSGlue> GetChildren()
        {
            return Enumerable.Empty<IJSCSGlue>();
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
