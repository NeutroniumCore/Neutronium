using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    internal class BulkJsHelper
    {
        internal IJavascriptObject BulkObjectsUpdater { get; }
        internal IJavascriptObject BulkArraysUpdater { get; }
        internal IJavascriptObject CommandConstructor { get; }
        internal IJavascriptObject ExecutableConstructor { get; }

        private readonly IJavascriptSessionCache _Cache;

        private readonly IJavascriptObject _CommandPrototype;
        private readonly IJavascriptObject _ExecutablePrototype;

        internal BulkJsHelper(IJavascriptSessionCache cache, IWebView webView, IJavascriptObject helper)
        {
            _Cache = cache;

            BulkObjectsUpdater = helper.GetValue("bulkUpdateObjects");
            BulkArraysUpdater = helper.GetValue("bulkUpdateArrays");
            CommandConstructor = helper.GetValue("Command");
            _CommandPrototype = CommandConstructor.GetValue("prototype");
            _CommandPrototype.Bind("privateExecute", webView, ExecuteExecutable);
            _CommandPrototype.Bind("privateCanExecute", webView, CanExecuteCommand);

            ExecutableConstructor = helper.GetValue("Executable");
            _ExecutablePrototype = ExecutableConstructor.GetValue("prototype");
            _ExecutablePrototype.Bind("privateExecute", webView, ExecuteExecutable);
        }

        private void ExecuteExecutable(IJavascriptObject[] arguments)
        {
            var executableGlue = GetFromArgument<IExecutableGlue>(arguments);
            executableGlue?.Execute(FilterAguments(arguments));
        }

        private void CanExecuteCommand(IJavascriptObject[] arguments)
        {
            var jsCommand = GetFromArgument<JsCommand>(arguments);
            jsCommand?.CanExecuteCommand(FilterAguments(arguments));
        }

        private static IJavascriptObject[] FilterAguments(IJavascriptObject[] arguments)
        {
            var result = new IJavascriptObject[arguments.Length - 1];
            for (var i = 1; i < arguments.Length; i++)
            {
                result[i - 1] = arguments[i];
            }
            return result;
        }

        private T GetFromArgument<T>(IJavascriptObject[] arguments) where T : class
        {
            var id = (uint)arguments[0].GetIntValue();
            return _Cache.GetCached(id) as T;
        }
    }
}
