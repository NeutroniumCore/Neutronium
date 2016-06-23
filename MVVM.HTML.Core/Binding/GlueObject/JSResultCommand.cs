using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.Component;
using MVVM.HTML.Core.Extension;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Binding.GlueObject
{
    public class JSResultCommand : GlueBase, IJSObservableBridge
    {
        private readonly IResultCommand _JSResultCommand;
        private readonly IWebView _WebView;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject JSValue { get; }
        public IJavascriptObject MappedJSValue => _MappedJSValue;
        public object CValue => _JSResultCommand;
        public JSCSGlueType Type => JSCSGlueType.ResultCommand;

        public JSResultCommand(IWebView webView, IJavascriptToCSharpConverter converter, IResultCommand resultCommand)
        {
            _WebView = webView;
            _JavascriptToCSharpConverter = converter;
            _JSResultCommand = resultCommand;
            JSValue = _WebView.Factory.CreateObject(true);
        }

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", _WebView, Execute);
        }
      
        private async void Execute(IJavascriptObject[] e)
        {
            var argument = _JavascriptToCSharpConverter.GetFirstArgumentOrNull(e);
            var promise = (e.Length > 1) ? e[1] : null;

            try
            {
                var res = await _JSResultCommand.Execute(argument);
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

            await _WebView.RunAsync(async () =>
            {
                var errormessage = (exception == null) ? "Faulted" : exception.Message;
                await promise.InvokeAsync("reject", _WebView, _WebView.Factory.CreateString(errormessage));
            });
        }

        private async Task SetResult(IJavascriptObject promise, object result)
        {
            if (promise == null)
                return;

            await _WebView.RunAsync(async () =>
            {
                var bridgevalue = await _JavascriptToCSharpConverter.RegisterInSession(result);
                await promise.InvokeAsync("fullfill", _WebView, bridgevalue.GetJSSessionValue());
            });
        }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return Enumerable.Empty<IJSCSGlue>();
        }

        protected override void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            sb.Append("{}");
        }
    }
}
