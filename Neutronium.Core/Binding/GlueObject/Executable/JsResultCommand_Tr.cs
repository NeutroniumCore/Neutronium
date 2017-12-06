using System.Threading.Tasks;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    internal class JsResultCommand<TResult> : JsResultCommandBase<TResult, bool>, IJsCsCachableGlue, IExecutableGlue
    {
        private readonly IResultCommand<TResult> _JsResultCommand;
        public object CValue => _JsResultCommand;
       
        public JsResultCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, IResultCommand<TResult> resultCommand):
            base(context, converter)
        {
            _JsResultCommand = resultCommand;
        }

        protected override IJavascriptObject GetPromise(IJavascriptObject[] e)
        {
            return (e.Length > 0) ? e[0] : null;
        }

        protected override async Task<Result<TResult>> ExecuteOnUiThread(bool context)
        {
            var res = await _JsResultCommand.Execute();
            return new Result<TResult>(res);
        }

        protected override Result<bool> ExecuteOnJsContextThread(IJavascriptObject[] e)
        {
            return new Result<bool>(true);
        }
    }
}
