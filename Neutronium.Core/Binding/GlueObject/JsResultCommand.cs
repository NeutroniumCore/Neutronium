using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neutronium.Core.Extension;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsResultCommand : GlueBase, IJSObservableBridge
    {
        private readonly IResultCommand _JSResultCommand;
        private readonly HTMLViewContext _HTMLViewContext;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject JSValue { get; private set; }
        public IJavascriptObject MappedJSValue => _MappedJSValue;
        public object CValue => _JSResultCommand;
        public JsCsGlueType Type => JsCsGlueType.ResultCommand;
        private IWebView WebView => _HTMLViewContext.WebView;
        private IDispatcher UIDispatcher => _HTMLViewContext.UIDispatcher;

        public JsResultCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, IResultCommand resultCommand)
        {
            _HTMLViewContext = context;
            _JavascriptToCSharpConverter = converter;
            _JSResultCommand = resultCommand;          
        }

        protected override bool LocalComputeJavascriptValue(IJavascriptObjectFactory factory)
        {
            if (JSValue != null)
                return false;

            JSValue = factory.CreateObject(true);
            return true;
        }

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", WebView, Execute);
        }
      
        private async void Execute(IJavascriptObject[] e)
        {
            var argument = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            var promise = (e.Length > 1) ? e[1] : null;

            try
            {
                var task = await UIDispatcher.EvaluateAsync(() => _JSResultCommand.Execute(argument));
                var res = await task;
                await SetResult(promise, res);
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

        private async Task SetResult(IJavascriptObject promise, object result)
        {
            if (promise == null)
                return;

            await WebView.RunAsync(async () =>
            {
                var bridgevalue = await _JavascriptToCSharpConverter.RegisterInSession(result);
                await promise.InvokeAsync("fullfill", WebView, bridgevalue.GetJSSessionValue());
            });
        }

        public override IEnumerable<IJSCSGlue> GetChildren()
        {
            return Enumerable.Empty<IJSCSGlue>();
        }

        protected override void ComputeString(NameContext context)
        {
            context.Append("{}");
        }
    }
}
