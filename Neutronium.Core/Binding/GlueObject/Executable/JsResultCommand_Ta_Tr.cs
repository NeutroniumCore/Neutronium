using System.Threading.Tasks;
using Neutronium.Core.Binding.Converter;
using Neutronium.Core.Extension;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    internal class JsResultCommand<TArg, TResult> : JsResultCommandBase<TResult, TArg>, IJsCsCachableGlue, IExecutableGlue
    {
        private readonly IResultCommand<TArg, TResult> _JsResultCommand;
        public object CValue => _JsResultCommand;

        public JsResultCommand(HtmlViewContext context, ICSharpUnrootedObjectManager manager, IJavascriptToCSharpConverter converter, IResultCommand<TArg, TResult> resultCommand) :
            base(context, manager, converter)
        {
            _JsResultCommand = resultCommand;
        }

        protected override IJavascriptObject GetPromise(IJavascriptObject[] e)
        {
            return (e.Length > 1) ? e[1] : null;
        }

        protected override MayBe<TArg> GetArgumentValueOnJsContext(IJavascriptObject[] e)
        {
            var argument = JavascriptToCSharpConverter.GetFirstArgument<TArg>(e);
            if (!argument.Success)
            {
                Logger.Error($"Impossible to call Execute on command<{typeof(TArg)}>, no matching argument found, received:{argument.TentativeValue} of type:{argument.TentativeValue?.GetType()} expectedType: {typeof(TArg)}");
            }
            return argument;
        }

        protected override Task<TResult> ExecuteOnUiThread(TArg argument)
        {
            return _JsResultCommand.Execute(argument);
        }
    }
}
