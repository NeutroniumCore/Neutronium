using System.Collections.Generic;
using System.Threading.Tasks;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.JavascriptFrameworkMapper
{
    internal class JavascriptFrameworkNoMapper : IJavascriptFrameworkMapper
    {
        private readonly ISessionCache _SessionCache;
        private readonly HtmlViewContext _Context;
        private readonly IJavascriptObjectBuilderStrategy _BuilderStrategy;

        internal JavascriptFrameworkNoMapper(HtmlViewContext context, ISessionCache sessionCache, IJavascriptObjectBuilderStrategy builderStrategy)
        {
            _Context = context;
            _SessionCache = sessionCache;
            _BuilderStrategy = builderStrategy;
        }

        public Task<IJavascriptObject> UpdateJavascriptObject(IJsCsGlue root)
        {
            return Task.FromResult(root?.JsValue);
        }

        public void UpdateOnJavascriptContext(BridgeUpdater updater, IList<IJsCsGlue> values)
        {
            values?.ForEach(UpdateJavascriptValue);
            UpdateOnJavascriptContext(updater);
        }

        public void UpdateOnJavascriptContext(BridgeUpdater updater, IJsCsGlue value)
        {
            if (value != null)
            {
                UpdateJavascriptValue(value);
            }
            UpdateOnJavascriptContext(updater);
        }

        private void UpdateJavascriptValue(IJsCsGlue value)
        {
            _BuilderStrategy.UpdateJavascriptValue(value);
        }

        public void UpdateOnJavascriptContext(BridgeUpdater updater)
        {
            updater.UpdateOnJavascriptContext(_Context.ViewModelUpdater, _SessionCache);
        }
    }
}
