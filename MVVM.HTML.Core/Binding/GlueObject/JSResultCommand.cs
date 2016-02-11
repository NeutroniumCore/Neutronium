using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.Component;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Binding.Mapping;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class JSResultCommand : GlueBase, IJSObservableBridge
    {
        private readonly IResultCommand _JSResultCommand;
        private readonly IWebView _IWebView;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        public JSResultCommand(IWebView ijsobject, IJavascriptToCSharpConverter converter, IResultCommand icValue)
        {
            _IWebView = ijsobject;
            _JavascriptToCSharpConverter = converter;
            _JSResultCommand = icValue;
            JSValue = _IWebView.Factory.CreateObject(true);
        }

        public IJavascriptObject JSValue { get; private set; }

        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", _IWebView,(c, o, e) => Execute(e));
        }

        private object Convert(IJavascriptObject value)
        {
            var found = _JavascriptToCSharpConverter.GetCachedOrCreateBasic(value, null);
            return (found != null) ? found.CValue : null;
        }

        private object GetArguments(IJavascriptObject[] e)
        {
            return (e.Length == 0) ? null : Convert(e[0]);
        }

        private void SetResult(IJavascriptObject[] e, Task<object> resulttask)
        {
            _IWebView.RunAsync (() =>
                 {
                     if (e.Length < 2)
                         return;

                     IJavascriptObject promise = e[1];
                     if (!resulttask.IsFaulted)
                     {
                         _JavascriptToCSharpConverter.RegisterInSession(resulttask.Result, (bridgevalue) =>
                         {
                             promise.InvokeAsync("fullfill", _IWebView, bridgevalue.GetJSSessionValue());
                         });
                     }
                     else
                     {
                         string error = (resulttask.IsCanceled) ? "Cancelled" :
                             ((resulttask.Exception == null) ? "Faulted" : resulttask.Exception.Flatten().InnerException.Message);

                         promise.InvokeAsync("reject", _IWebView, _IWebView.Factory.CreateString(error));
                     }
                 });
        }

        private void Execute(IJavascriptObject[] e)
        {
            _JSResultCommand.Execute(GetArguments( e))
                .ContinueWith(t => SetResult(e, t));
        }

        public object CValue
        {
            get { return _JSResultCommand; }
        }

        public JSCSGlueType Type
        {
            get { return JSCSGlueType.ResultCommand; }
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
