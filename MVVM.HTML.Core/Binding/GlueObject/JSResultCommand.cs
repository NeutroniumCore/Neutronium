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
        private IResultCommand _JSResultCommand;
        private IWebView _IWebView;
        public JSResultCommand(IWebView ijsobject, IResultCommand icValue)
        {
            _IWebView = ijsobject;
            _JSResultCommand = icValue;
            JSValue = _IWebView.Factory.CreateObject(true);
        }

        public IJavascriptObject JSValue { get; private set; }

        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(IJavascriptObject ijsobject, IJavascriptToCSharpConverter mapper)
        {
            _MappedJSValue = ijsobject;
            _MappedJSValue.Bind("Execute", _IWebView,(c, o, e) => Execute(e, mapper));
        }

        private object Convert(IJavascriptToCSharpConverter mapper, IJavascriptObject value)
        {
            var found = mapper.GetCachedOrCreateBasic(value, null);
            return (found != null) ? found.CValue : null;
        }

        private object GetArguments(IJavascriptToCSharpConverter mapper, IJavascriptObject[] e)
        {
            return (e.Length == 0) ? null : Convert(mapper, e[0]);
        }

        private void SetResult(IJavascriptObject[] e, IJavascriptToCSharpConverter bridge, Task<object> resulttask)
        {
            _IWebView.RunAsync (() =>
                 {
                     if (e.Length < 2)
                         return;

                     IJavascriptObject promise = e[1];
                     if (!resulttask.IsFaulted)
                     {
                         bridge.RegisterInSession(resulttask.Result, (bridgevalue) =>
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

        private void Execute(IJavascriptObject[] e, IJavascriptToCSharpConverter mapper)
        {
            _JSResultCommand.Execute(GetArguments(mapper, e))
                .ContinueWith(t => SetResult(e, mapper, t));
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
