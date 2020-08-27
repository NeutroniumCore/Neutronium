using System.Collections.Generic;
using System.Threading.Tasks;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.SessionManagement;
using Neutronium.Core.Exceptions;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.JavascriptFrameworkMapper
{
    internal class JavascriptFrameworkRealMapper : IJavascriptFrameworkMapper
    {
        private readonly IInternalSessionCache _SessionCache;
        private readonly HtmlViewContext _Context;
        private readonly IJavascriptObjectBuilderStrategy _BuilderStrategy;
        private readonly IJavascriptSessionInjector _SessionInjector;

        internal JavascriptFrameworkRealMapper(HtmlViewContext context, IJavascriptSessionInjector sessionInjector, IInternalSessionCache sessionCache, IJavascriptObjectBuilderStrategy builderStrategy)
        {
            _SessionInjector = sessionInjector;
            _SessionCache = sessionCache;
            _BuilderStrategy = builderStrategy;
            _Context = context;
        }

        public async Task<IJavascriptObject> UpdateJavascriptObject(IJsCsGlue root)
        {
            if ((root == null) || (root.IsBasic()))
                return null;

            var jvm = _SessionCache.GetMapper(root as IJsCsMappedBridge);
            var res = _SessionInjector.Inject(root.JsValue, jvm);

            if ((root.CValue != null) && (res == null))
            {
                throw ExceptionHelper.GetUnexpected();
            }

            await jvm.UpdateTask;
            return res;
        }

        public async void UpdateOnJavascriptContext(BridgeUpdater updater, IList<IJsCsGlue> values)
        {
            if (values != null && values.Count > 0)
            {
                values.ForEach(UpdateJavascriptValue);
                foreach (var jsCsGlue in values)
                {
                    await UpdateJavascriptObject(jsCsGlue);
                }
            }

            UpdateOnJavascriptContext(updater);
        }

        public async void UpdateOnJavascriptContext(BridgeUpdater updater, IJsCsGlue value)
        {
            if (value != null)
            {
                UpdateJavascriptValue(value);
                await UpdateJavascriptObject(value);
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
