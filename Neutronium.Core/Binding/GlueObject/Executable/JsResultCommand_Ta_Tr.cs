using System.Threading.Tasks;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    internal class JsResultCommand<TArg, TResult> : JsResultCommandBase<TResult, TArg>, IJsCsCachableGlue, IExecutableGlue
    {
        private readonly IResultCommand<TArg, TResult> _JsResultCommand;
        public object CValue => _JsResultCommand;

        public JsResultCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, IResultCommand<TArg, TResult> resultCommand) :
            base(context, converter)
        {
            _JsResultCommand = resultCommand;
        }

        protected override IJavascriptObject GetPromise(IJavascriptObject[] e)
        {
            return (e.Length > 1) ? e[1] : null;
        }

        protected override Result<TArg> ExecuteOnJsContextThread(IJavascriptObject[] e)
        {
            var argument = JavascriptToCSharpConverter.GetFirstArgument<TArg>(e);
            if (!argument.Success)
            {
                Logger.Error($"Impossible to call Execute on command<{typeof(TArg)}>, no matching argument found, received:{argument.TentativeValue} of type:{argument.TentativeValue?.GetType()} expectedType: {typeof(TArg)}");
            }
            return argument;
        }

        protected override async Task<Result<TResult>> ExecuteOnUiThread(TArg argument)
        {
            var value = await _JsResultCommand.Execute(argument);
            return new Result<TResult>(value);
        }
    }
}
