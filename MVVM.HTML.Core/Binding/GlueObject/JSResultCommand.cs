using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using MVVM.Component;
using MVVM.HTML.Core.Binding.Extension;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Infra;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class JSResultCommand : GlueBase, IJSObservableBridge
    {
        private readonly IResultCommand _JSResultCommand;
        private readonly IWebView _IWebView;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject JSValue { get; private set; }
        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }
        public object CValue { get { return _JSResultCommand; } }
        public JSCSGlueType Type { get { return JSCSGlueType.ResultCommand; } }

        public JSResultCommand(IWebView ijsobject, IJavascriptToCSharpConverter converter, IResultCommand icValue)
        {
            _IWebView = ijsobject;
            _JavascriptToCSharpConverter = converter;
            _JSResultCommand = icValue;
            JSValue = _IWebView.Factory.CreateObject(true);
        }

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", _IWebView, Execute);
        }
      
        private async void Execute(IJavascriptObject[] e)
        {
            if (e.Length ==0)
                return;

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

            await _IWebView.RunAsync(async () =>
            {
                var errormessage = (exception == null) ? "Faulted" : exception.Message;
                await promise.InvokeAsync("reject", _IWebView, _IWebView.Factory.CreateString(errormessage));
            });
        }

        private async Task SetResult(IJavascriptObject promise, object result)
        {
            if (promise == null)
                return;

            await _IWebView.RunAsync(async () =>
            {
                var bridgevalue = await _JavascriptToCSharpConverter.RegisterInSession(result);
                await promise.InvokeAsync("fullfill", _IWebView, bridgevalue.GetJSSessionValue());
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
