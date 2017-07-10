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

        private readonly Lazy<IJavascriptObject> _Helper;
        private readonly Lazy<IJavascriptObject> _BulkPropertyCreator;
        private readonly Lazy<IJavascriptObject> _BulkPArrayCreator;

        public JavascriptObjectBulkBuilderStrategy(IWebView webView, IJavascriptSessionCache cache)
        {
            _WebView = webView;
            _Cache = cache;
            _Helper = new Lazy<IJavascriptObject>(HelperBuilder);
            _BulkPropertyCreator = new Lazy<IJavascriptObject>(BulkPropertyCreatorBuilder);
            _BulkPArrayCreator = new Lazy<IJavascriptObject>(BulkArrayCreatorBuilder);
        }

        public void UpdateJavascriptValue(IJSCSGlue root)
        {
            var builder = new JavascriptObjectBulkBuilder(_WebView.Factory, _Cache, this, root);
            builder.UpdateJavascriptValue();
        }

        private IJavascriptObject HelperBuilder()
        {
            var script =
                @"(function(){
                    function bulkCreateProperty(prop){
                        const propss = eval(prop)
                        const count = propss.length
		                const args = Array.from(arguments)
		                const objs = args.slice(1, count + 1)
		                const values = args.slice(1 + count, args.length + 1)
                        var valueCount = 0
		                for(var i=0; i< count; i ++){
                             var props = propss[i]
                             for (var j = 0, len = props.length; j < len; j++) {
                                objs[i][props[j]] = values[valueCount++]
                             }
		                }
                    }

                    function bulkCreateArray(countsString){
                        const counts = eval(countsString)
                        const length = counts.length
		                const args = Array.from(arguments)
		                const objs = args.slice(1, length + 1)
		                const values = args.slice(1 + length, args.length + 1)
                        var valueCount = 0
                        const push = Array.prototype.push
		                for(var i=0; i< length; i ++) {
                            var nextCount = valueCount + counts[i]
                            push.apply(objs[i], values.slice(valueCount, nextCount))
                            valueCount = nextCount
		                }
                    }
                    return {
                        bulkCreateProperty,
                        bulkCreateArray
                    }
                })()";

            IJavascriptObject helper;
            _WebView.Eval(script, out helper);           
            return helper;
        }

        private IJavascriptObject GetProperty(string atttibute) => _Helper.Value.GetValue(atttibute);
        private IJavascriptObject BulkPropertyCreatorBuilder() => GetProperty("bulkCreateProperty");
        private IJavascriptObject BulkArrayCreatorBuilder() => GetProperty("bulkCreateArray");

        void IBulkUpdater.BulkUpdateProperty(List<Tuple<IJSCSGlue, IReadOnlyDictionary<string, IJSCSGlue>>> updates)
        {
            if (updates.Count == 0)
                return;

            SplitParameters(updates, dic => dic.Count).Select(GetUpdatePropertyParameters)
                .ForEach(arguments => Execute(_BulkPropertyCreator, arguments));
        }

        private IJavascriptObject[] GetUpdatePropertyParameters(List<Tuple<IJSCSGlue, IReadOnlyDictionary<string, IJSCSGlue>>> updates)
        {
            var properties = AsArray(updates.Select(up => AsArray(up.Item2.Keys.Select(p => $"'{p}'"))));
            var objects = updates.Select(up => up.Item1);
            var values = updates.SelectMany(up => up.Item2.Values);

            return BuildArguments(properties, objects.Concat(values));
        }

        //private const int MaxCount = 250000;
        private const int MaxCount = 200000;

        private IEnumerable<List<Tuple<IJSCSGlue,T>>> SplitParameters<T>(List<Tuple<IJSCSGlue,T>> paramBuilder, Func<T,int> counter, int addicionalParameters = 1)
        {
            var index = 0;
            var maxCount = paramBuilder.Count;
            do
            {
                var count = addicionalParameters;
                var paramsToCompute = paramBuilder.Skip(index);
                var collection = paramsToCompute.Select(p => counter(p.Item2)).TakeWhile(p => { count += p + 1; return count < MaxCount; });
                var localCount = collection.Count();
                if (localCount == 0)
                    yield break;

                yield return paramBuilder.Skip(index).Take(localCount).ToList();

                index += localCount;
            }
            while (index < maxCount);
        }

        void IBulkUpdater.BulkUpdateArray(List<Tuple<IJSCSGlue, IList<IJSCSGlue>>> updates)
        {
            if (updates.Count == 0)
                return;

            var sizes = AsArray(updates.Select(item => item.Item2.Count.ToString()));
            var objects = updates.Select(up => up.Item1);
            var values = updates.SelectMany(up => up.Item2);

            var arguments = BuildArguments(sizes, objects.Concat(values));
            Execute(_BulkPArrayCreator, arguments);
        }

        private static string AsArray(IEnumerable<string> value) => $"[{string.Join(",", value)}]";

        private IJavascriptObject[] BuildArguments(string paramString, IEnumerable<IJSCSGlue> paramsObjects)
        {
            return new[] { _WebView.Factory.CreateString(paramString) }.Concat(paramsObjects.Select(glue => glue.JSValue)).ToArray();
        }

        private void Execute(Lazy<IJavascriptObject> @function, params IJavascriptObject[] arguments)
        {
            @function.Value.ExecuteFunctionNoResult(_WebView, null, arguments);
        }
    }
}
