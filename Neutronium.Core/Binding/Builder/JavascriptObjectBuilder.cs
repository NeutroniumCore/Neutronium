using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectBuilder: IBulkUpdater
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;

        private readonly Lazy<IJavascriptObject> _Helper;
        private readonly Lazy<IJavascriptObject> _BulkPropertyCreator;
        private readonly Lazy<IJavascriptObject> _BulkPArrayCreator;

        public JavascriptObjectBuilder(IWebView webView, IJavascriptSessionCache cache)
        {
            _WebView = webView;
            _Cache = cache;
            _Helper = new Lazy<IJavascriptObject>(HelperBuilder);
            _BulkPropertyCreator = new Lazy<IJavascriptObject>(BulkPropertyCreatorBuilder);
            _BulkPArrayCreator = new Lazy<IJavascriptObject>(BulkArrayCreatorBuilder);
        }

        public void UpdateJavascriptValue(IJSCSGlue root)
        {
            var builder = new JavascriptObjectOneShotBuilder(_WebView.Factory, _Cache, this, root);
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
                    return {
                        bulkCreateProperty: bulkCreateProperty,
                        pusher: [].push
                    }
                })()";

            IJavascriptObject helper;
            _WebView.Eval(script, out helper);           
            return helper;
        }

        private IJavascriptObject BulkArrayCreatorBuilder()
        {
            return _Helper.Value.GetValue("pusher");
        }

        private IJavascriptObject BulkPropertyCreatorBuilder()
        {
            return _Helper.Value.GetValue("bulkCreateProperty");
        }

        void IBulkUpdater.BulkUpdateProperty(List<Tuple<IJSCSGlue, IReadOnlyDictionary<string, IJSCSGlue>>> updates)
        {
            if (updates.Count == 0)
                return;

            var properties = AsArray(string.Join(",", updates.Select(up => AsArray(string.Join(",", up.Item2.Keys.Select(p => $"'{p}'"))))));
            var objects = updates.Select(up => up.Item1);
            var values = updates.SelectMany(up => up.Item2.Values);

            var paramsObjects = objects.Concat(values).Select(glue => glue.JSValue);

            var arguments = new[] { _WebView.Factory.CreateString(properties) }
                            .Concat(paramsObjects)
                            .ToArray();

            _BulkPropertyCreator.Value.ExecuteFunctionNoResult(_WebView, null, arguments);
        }

        private static string AsArray(string value) => $"[{value}]";

        public void BulkUpdateArray(List<Tuple<IJSCSGlue, IList<IJSCSGlue>>> updates)
        {
            if (updates.Count == 0)
                return;

            var pusher = _BulkPArrayCreator.Value;
            foreach (var arrayUpdate in updates)
            {
                var children = arrayUpdate.Item2;
                var jsValue = arrayUpdate.Item1.JSValue;
                var dest = children.Select(v => v.JSValue).ToArray();
                pusher.ExecuteFunctionNoResult(_WebView, jsValue, dest);
            }
        }
    }
}
