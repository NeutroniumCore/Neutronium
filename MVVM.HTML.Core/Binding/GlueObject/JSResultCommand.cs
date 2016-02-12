using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.Component;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Binding.Mapping;

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
        public JSCSGlueType Type  { get { return JSCSGlueType.ResultCommand; }  }

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
            _MappedJSValue.Bind("Execute", _IWebView,(c, o, e) => Execute(e));
        }

        private void SetResult(IJavascriptObject[] e, Task<object> resulttask)
        {
            _IWebView.RunAsync (() =>
                 {
                     if (e.Length < 2)
                         return;

                     var promise = e[1];
                     if (!resulttask.IsFaulted)
                     {
                         _JavascriptToCSharpConverter.RegisterInSession(resulttask.Result, (bridgevalue) =>
                         {
                             promise.InvokeAsync("fullfill", _IWebView, bridgevalue.GetJSSessionValue());
                         });
                     }
                     else
                     {
                         var errormessage = (resulttask.IsCanceled) ? "Cancelled" :
                             ((resulttask.Exception == null) ? "Faulted" : resulttask.Exception.Flatten().InnerException.Message);

                         promise.InvokeAsync("reject", _IWebView, _IWebView.Factory.CreateString(errormessage));
                     }
                 });
        }

        private void Execute(IJavascriptObject[] e)
        {
            _JSResultCommand.Execute(_JavascriptToCSharpConverter.GetArguments(e))
                .ContinueWith(t => SetResult(e, t));
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
