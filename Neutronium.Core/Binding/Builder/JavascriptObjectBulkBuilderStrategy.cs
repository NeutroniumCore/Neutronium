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
                    return {
                        bulkCreate
                    }
                })()";

            IJavascriptObject helper;
            _WebView.Eval(script, out helper);
            return helper.GetValue("bulkCreate");
        }

        void IBulkUpdater.BulkUpdateProperty(List<ChildrenPropertyDescriptor> updates)
        {
            BulkUpdate(updates, key => $"'{key}'");
        }

        void IBulkUpdater.BulkUpdateArray(List<ChildrenArrayDescriptor> updates)
        {
            BulkUpdate(updates, key => $"{key}");
        }

        private void BulkUpdate<T>(IEnumerable<ChildrenDescriptor<T>> updates, Func<T, string> getKeyDescription)
        {
            if (updates.Count() == 0)
                return;

            var spliter = new Spliter<T> { MaxCount = _WebView.MaxFunctionArgumentsNumber -1 };

            spliter.SplitParameters(updates)
                    .Select(param => GetUpdateParameters(param, getKeyDescription))
                    .ForEach(arguments => Execute(arguments));
        }

        private IJavascriptObject[] GetUpdateParameters<T>(List<ChildrenDescriptor<T>> updates, Func<T,string> getKeyDescription)
        {
            var sizes = AsArray(updates.Select(item => AsArray(item.ChildrenDescription.Select(p => getKeyDescription(p.Key)))));
            var objects = updates.Select(up => up.Father);
            var values = updates.SelectMany(up => up.ChildrenDescription).Select(desc => desc.Child);
            return BuildArguments(sizes, objects.Concat(values));
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
