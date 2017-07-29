using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectBulkBuilderStrategy : IJavascriptObjectBuilderStrategy, IBulkUpdater
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;
        private readonly Lazy<IJavascriptObject> _BulkCreator;

        public JavascriptObjectBulkBuilderStrategy(IWebView webView, IJavascriptSessionCache cache)
        {
            _WebView = webView;
            _Cache = cache;
            _BulkCreator = new Lazy<IJavascriptObject>(BulkCreatorBuilder);
        }

        public void UpdateJavascriptValue(IJSCSGlue root)
        {
            var builder = new JavascriptObjectBulkBuilder(_WebView.Factory, _Cache, this, root);
            builder.UpdateJavascriptValue();
        }

        private IJavascriptObject BulkCreatorBuilder()
        {
            var script =
                 @"(function(){
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
                        bulkCreate
                    }
                })()";

            IJavascriptObject helper;
            _WebView.Eval(script, out helper);
            return helper.GetValue("bulkCreate");
        }

        void IBulkUpdater.BulkUpdateProperty(List<EntityDescriptor<string>> updates)
        {
            var orderedUpdates = updates.GroupBy(up => up.Father.GetType()).SelectMany(grouped => grouped);
            BulkUpdate(orderedUpdates, key => $@"""{key}""");
        }

        void IBulkUpdater.BulkUpdateArray(List<EntityDescriptor<int>> updates)
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
