using System.Threading.Tasks;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Binding.SessionManagement;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    internal class JsResultCommand<TResult> : JsResultCommandBase<TResult, bool>, IJsCsCachableGlue, IExecutableGlue
    {
        private readonly IResultCommand<TResult> _JsResultCommand;
        public object CValue => _JsResultCommand;
       
        public JsResultCommand(HtmlViewContext context, ICSharpUnrootedObjectManager manager, IJavascriptToGlueMapper converter, IResultCommand<TResult> resultCommand):
            base(context, manager, converter)
        {
            _JsResultCommand = resultCommand;
        }

        protected override IJavascriptObject GetPromise(IJavascriptObject[] e)
        {
            return (e.Length > 0) ? e[0] : null;
        }

        protected override Task<TResult> ExecuteOnUiThread(bool context)
        {
            return _JsResultCommand.Execute();
        }

        protected override MayBe<bool> GetArgumentValueOnJsContext(IJavascriptObject[] e)
        {
            return new MayBe<bool>(true);
        }
    }
}
