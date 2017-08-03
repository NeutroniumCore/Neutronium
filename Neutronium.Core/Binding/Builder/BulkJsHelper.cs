using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Extension;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class BulkJsHelper
    {
        internal IJavascriptObject BulkCreator { get; }
        internal IJavascriptObject CommandConstructor { get; }
        internal IJavascriptObject ExecutableConstructor { get; }

        private readonly IJavascriptSessionCache _Cache;
        private readonly IWebView _WebView;

        private readonly IJavascriptObject _CommandPrototype;
        private readonly IJavascriptObject _ExecutablePrototype;

        internal BulkJsHelper(IJavascriptSessionCache cache, IWebView webView, IJavascriptObject helper)
        {
            _Cache = cache;
            _WebView = webView;

            BulkCreator = helper.GetValue("bulkCreate");
            CommandConstructor = helper.GetValue("Command");
            _CommandPrototype = CommandConstructor.GetValue("prototype");
            _CommandPrototype.Bind("privateExecute", _WebView, ExecuteExecutable);
            _CommandPrototype.Bind("privateCanExecute", _WebView, CanExecuteCommand);

            ExecutableConstructor = helper.GetValue("Executable");
            _ExecutablePrototype = ExecutableConstructor.GetValue("prototype");
            _ExecutablePrototype.Bind("privateExecute", _WebView, ExecuteExecutable);
        }

        private void ExecuteExecutable(IJavascriptObject[] arguments)
        {
            var executableGlue = GetFromArgument<IExecutableGlue>(arguments);
            executableGlue?.Execute(arguments.Skip(1).ToArray());
        }

        private void CanExecuteCommand(IJavascriptObject[] arguments)
        {
            var jsCommand = GetFromArgument<JSCommand>(arguments);
            jsCommand?.CanExecuteCommand(arguments.Skip(1).ToArray());
        }

        private T GetFromArgument<T>(IJavascriptObject[] arguments) where T: class
        {
            var id = (uint)arguments[0].GetIntValue();
            return _Cache.GetCached(id) as T;
        }
    }
}
