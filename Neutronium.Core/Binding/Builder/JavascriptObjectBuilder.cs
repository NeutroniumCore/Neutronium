using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectBuilder: IBulkPropertyUpdater
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;
        private readonly Lazy<IJavascriptObject> _BulkPropertyCreator;

        public JavascriptObjectBuilder(IWebView webView, IJavascriptSessionCache cache)
        {
            _WebView = webView;
            _Cache = cache;
            _BulkPropertyCreator = new Lazy<IJavascriptObject>(BulkPropertyCreatorBuilder);
        }

        private IJavascriptObject BulkPropertyCreatorBuilder()
        {
            var script =
                @"(function(){
                    function bulkCreateProperty(prop){
                        var props = eval(prop)
                        var count = props.length
		                var args = [].slice.call(arguments)
		                var objs = args.slice(1, count+1)
		                var values = args.slice(1+ count, args.length +1)
		                for(var i=0; i< count; i ++){
                             objs[i][props[i]] = values[i]
		                }
                    }
                    return {
                        bulkCreateProperty: bulkCreateProperty
                    }
                })()";

            IJavascriptObject obj;
            _WebView.Eval(script, out obj);
            var function = obj.GetValue("bulkCreateProperty");
            return function;
        }

        void IBulkPropertyUpdater.BulkUpdateProperty(List<Tuple<IJSCSGlue, IReadOnlyDictionary<string, IJSCSGlue>>> updates)
        {
            if (updates.Count == 0)
                return;

            var properties = $"[{string.Join(",", updates.SelectMany(up => up.Item2.Keys).Select(p => $"'{p}'"))}]";
            var objects = updates.SelectMany(up => Enumerable.Repeat(up.Item1, up.Item2.Count));
            var values = updates.SelectMany(up => up.Item2.Values);

            var paramsObjects = objects.Concat(values).Select(glue => glue.JSValue);

            var arguments = new[] { _WebView.Factory.CreateString(properties) }
                            .Concat(paramsObjects)
                            .ToArray();

            _BulkPropertyCreator.Value.ExecuteFunctionNoResult(_WebView, null, arguments);
        }

        public void UpdateJavascriptValue(IJSCSGlue root)
        {
            var builder = new JavascriptObjectOneShotBuilder(_WebView, _Cache, this, root);
            builder.UpdateJavascriptValue();
        }
    }
}
