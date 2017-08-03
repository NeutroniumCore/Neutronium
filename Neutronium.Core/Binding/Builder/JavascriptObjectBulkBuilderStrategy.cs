using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Extension;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectBulkBuilderStrategy : IJavascriptObjectBuilderStrategy, IBulkUpdater
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;
        private readonly Lazy<IJavascriptObject> _Factory;
        private readonly Lazy<IJavascriptObject> _BulkCreator;
        private readonly Lazy<IJavascriptObject> _CommandConstructor;
        private IJavascriptObject _CommandPrototype;  
        private readonly bool _Mapping;

        public IJavascriptObject CommandConstructor => _CommandConstructor.Value;

        public JavascriptObjectBulkBuilderStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping)
        {
            _Mapping = mapping;
            _WebView = webView;
            _Cache = cache;
            _Factory = new Lazy<IJavascriptObject>(FactoryBuilder);
            _BulkCreator = new Lazy<IJavascriptObject>(BulkCreatorBuilder);
            _CommandConstructor = new Lazy<IJavascriptObject>(CommandConstructorBuilder);
        }

        public void UpdateJavascriptValue(IJSCSGlue root)
        {
            var builder = new JavascriptObjectBulkBuilder(_WebView.Factory, _Cache, this, root, _Mapping);
            builder.UpdateJavascriptValue();
        }

        private IJavascriptObject FactoryBuilder()
        {
            var script =
                 @"(function(){
                    function Command(id, canExecute){
                        Object.defineProperty(this, '{{NeutroniumConstants.ObjectId}}', {value: id});
                        Object.defineProperty(this, '{{NeutroniumConstants.ReadOnlyFlag}}', {value: true});
                        this.CanExecuteCount = 1;
                        this.CanExecuteValue = canExecute;
                    }
                    Command.prototype.Execute = function(arg) {
                        this.privateExecute(this.{{NeutroniumConstants.ObjectId}}, arg)
                    }
                    Command.prototype.CanExecute = function(arg) {
                        this.privateCanExecute(this.{{NeutroniumConstants.ObjectId}}, arg)
                    }
                    function bulkCreate(prop){
                        const propss = JSON.parse(prop)
                        const count = propss.count
		                const args = Array.from(arguments)
		                const objs = args.slice(1, count + 1)
		                const values = args.slice(1 + count, args.length + 1)
                        var valueCount = 0
                        var elementCount = 0
                        var innerCount = 0
                        const elements = propss.elements
                        var element = null
		                for(var i=0; i< count; i ++){
                            if (!element || innerCount > element.c) {
                                element = elements[elementCount++]
                                innerCount = 0;
                            }
                            var props = element.a
                            for (var j = 0, len = props.length; j < len; j++) {
                                objs[i][props[j]] = values[valueCount++]
                            }
                            innerCount++
		                }
                    }
                    return {
                        bulkCreate,
                        Command
                    }
                })()";

            IJavascriptObject helper;
            script = script.Replace("{{NeutroniumConstants.ObjectId}}", NeutroniumConstants.ObjectId)
                            .Replace("{{NeutroniumConstants.ReadOnlyFlag}}", NeutroniumConstants.ReadOnlyFlag);
            _WebView.Eval(script, out helper);
            return helper;
        }

        private IJavascriptObject GetProperty(string atttibute) => _Factory.Value.GetValue(atttibute);
        private IJavascriptObject BulkCreatorBuilder() => GetProperty("bulkCreate");

        private IJavascriptObject CommandConstructorBuilder()
        {
            var command = GetProperty("Command");
            _CommandPrototype = command.GetValue("prototype");
            _CommandPrototype.Bind("privateExecute", _WebView, ExecuteCommand);
            _CommandPrototype.Bind("privateCanExecute", _WebView, CanExecuteCommand);
            return command;
        }

        private void CanExecuteCommand(IJavascriptObject[] arguments)
        {
            var jsCommand = GetFromArgument(arguments);
            jsCommand?.CanExecuteCommand(arguments[1]);
        }

        private void ExecuteCommand(IJavascriptObject[] arguments)
        {
            var jsCommand = GetFromArgument(arguments);
            jsCommand?.ExecuteCommand(arguments[1]);
        }

        private JSCommand GetFromArgument(IJavascriptObject[] arguments)
        {
            var id = (uint)arguments[0].GetIntValue();
            return _Cache.GetCached(id) as JSCommand;
        }

        void IBulkUpdater.BulkUpdateProperty(IEnumerable<EntityDescriptor<string>> updates)
        {
            var orderedUpdates = updates.GroupBy(up => up.Father.GetType()).SelectMany(grouped => grouped);
            BulkUpdate(orderedUpdates, key => $@"""{key}""");
        }

        void IBulkUpdater.BulkUpdateArray(IEnumerable<EntityDescriptor<int>> updates)
        {
            BulkUpdate(updates, key => $"{key}");
        }

        private void BulkUpdate<T>(IEnumerable<EntityDescriptor<T>> updates, Func<T, string> getKeyDescription)
        {
            if (updates.Count() == 0)
                return;

            var spliter = new EntityDescriptorSpliter<T> { MaxCount = _WebView.MaxFunctionArgumentsNumber -1 };

            spliter.SplitParameters(updates)
                    .Select(param => GetUpdateParameters(param, getKeyDescription))
                    .ForEach(arguments => Execute(arguments));
        }

        private IJavascriptObject[] GetUpdateParameters<T>(List<EntityDescriptor<T>> updates, Func<T,string> getKeyDescription)
        {
            var sizes = Pack(updates, getKeyDescription);
            var objects = updates.Select(up => up.Father);
            var values = updates.SelectMany(up => up.ChildrenDescription).Select(desc => desc.Child);
            return BuildArguments(sizes, objects.Concat(values));
        }

        private static string Pack<T>(List<EntityDescriptor<T>> updates, Func<T, string> getKeyDescription)
        {
            return $@"{{""count"":{updates.Count},""elements"":{AsArray(PackKeys(updates)
                                    .Select(pack => $@"{{""c"":{pack.Item1},""a"":{AsArray(pack.Item2.Select(getKeyDescription))}}}"))}}}" ;
         }

        private static IEnumerable<Tuple<int, IReadOnlyCollection<T>>> PackKeys<T>(List<EntityDescriptor<T>> updates)
        {
            var count = 0;
            IReadOnlyCollection<T> childrenKeys = null;
            foreach (var description in updates.Select(up => up.ChildrenDescription))
            {
                var keys = description.Select(desc => desc.Key);
                if (childrenKeys == null)
                {
                    childrenKeys = keys.ToList();
                    continue;
                }
                if (childrenKeys.SequenceEqual(keys))
                {
                    count++;
                    continue;
                }

                yield return Tuple.Create(count, childrenKeys);
                childrenKeys = keys.ToList();
                count = 0;
            }
            if (childrenKeys == null)
                yield break;

            yield return Tuple.Create(count, childrenKeys);
        }

        private IJavascriptObject[] BuildArguments(string paramString, IEnumerable<IJSCSGlue> paramsObjects)
        {
            return new[] { _WebView.Factory.CreateString(paramString) }.Concat(paramsObjects.Select(glue => glue.JSValue)).ToArray();
        }

        private static string AsArray(IEnumerable<string> value) => $"[{string.Join(",", value)}]";

        private void Execute(params IJavascriptObject[] arguments)
        {
            _BulkCreator.Value.ExecuteFunctionNoResult(_WebView, null, arguments);
        }
    }
}
