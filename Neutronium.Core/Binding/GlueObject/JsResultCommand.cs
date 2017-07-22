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
    public class JsResultCommand : GlueBase, IJSObservableBridge
    {
        private readonly IResultCommand _JSResultCommand;
        private readonly HTMLViewContext _HTMLViewContext;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject MappedJSValue => _MappedJSValue;
        public object CValue => _JSResultCommand;
        public JsCsGlueType Type => JsCsGlueType.ResultCommand;
        private IWebView WebView => _HTMLViewContext.WebView;
        private IDispatcher UIDispatcher => _HTMLViewContext.UIDispatcher;

        private uint _JsId;
        public uint JsId => _JsId;
        void IJSObservableBridge.SetJsId(uint jsId) => _JsId = jsId;

        public JsResultCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, IResultCommand resultCommand)
        {
            _HTMLViewContext = context;
            _JavascriptToCSharpConverter = converter;
            _JSResultCommand = resultCommand;
        }

        public void GetBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestObjectCreation();
        }

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {
            _MappedJSValue = jsobject;
            _MappedJSValue.Bind("Execute", WebView, Execute);
        }
      
        private async void Execute(IJavascriptObject[] e)
        {
            var argument = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            var promise = (e.Length > 1) ? e[1] : null;

            try
            {
                var task = await UIDispatcher.EvaluateAsync(async () =>
                {
                    var res = await _JSResultCommand.Execute(argument);
                    return (res==null) ? null :await _JavascriptToCSharpConverter.RegisterInSession(res);
                });
                var bridgevalue = await task;
                await SetResult(promise, bridgevalue);
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

        private async Task SetResult(IJavascriptObject promise, IJSCSGlue bridgevalue)
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
