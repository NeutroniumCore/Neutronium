using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Infra;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Neutronium.Core.Binding.Updaters
{
    internal class JsUpdater : IJsUpdater
    {
        private readonly HtmlViewContext _Context;
        private readonly CSharpToJavascriptConverter _JsObjectBuilder;
        private readonly ObjectChangesListener _Off;
        private readonly Lazy<IJavascriptObjectBuilderStrategy> _BuilderStrategy;
        private readonly ISessionMapper _SessionMapper;
        private readonly SessionCacher _SessionCache;

        internal JsUpdater(ISessionMapper sessionMapper, HtmlViewContext context, Func<IJavascriptObjectBuilderStrategy> strategy, CSharpToJavascriptConverter builder, ObjectChangesListener off, SessionCacher sessionCache)
        {
            _SessionMapper = sessionMapper;
            _Context = context;
            _BuilderStrategy = new Lazy<IJavascriptObjectBuilderStrategy>(strategy);
            _JsObjectBuilder = builder;
            _SessionCache = sessionCache;
            _Off = off;
        }

        public IJavascriptUpdater GetUpdaterForPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            return new PropertyJavascriptUpdater(this, sender, e);
        }

        public IJavascriptUpdater GetUpdaterForNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            return new CollectionJavascriptUpdater(this, sender, e);
        }

        public T GetCached<T>(object value) where T : class, IJsCsGlue
        {
            return _SessionCache.GetCached(value) as T;
        }

        public IJsCsGlue Map(object value)
        {
            return _JsObjectBuilder.Map(value);
        }

        public BridgeUpdater UpdateBridgeUpdater(BridgeUpdater value)
        {
            value.Cache = _SessionCache;
            value.CleanAfterChangesOnUiThread(_Off);
            return value;
        }

        public void UpdateOnJavascriptContextAllContext(BridgeUpdater updater, IJsCsGlue value)
        {
            if (value == null)
            {
                RunUpdaterOnJsContext(updater);
                return;
            }

            if (_Context.JavascriptFrameworkIsMappingObject)
            {
                UpdateOnJavascriptContextWithMapping(updater, value).DoNotWait();
                return;
            }

            UpdateOnJavascriptContextWithoutMapping(updater, value);
        }

        private void UpdateOnJavascriptContextWithoutMapping(BridgeUpdater updater, IJsCsGlue value)
        {
            _BuilderStrategy.Value.UpdateJavascriptValue(value);
            RunUpdaterOnJsContext(updater);
        }

        private async Task UpdateOnJavascriptContextWithMapping(BridgeUpdater updater, IJsCsGlue value)
        {
            _BuilderStrategy.Value.UpdateJavascriptValue(value);
            await _SessionMapper.InjectInHtmlSession(value);
            RunUpdaterOnJsContext(updater);
        }

        private void RunUpdaterOnJsContext(BridgeUpdater updater)
        {
            updater.UpdateOnJavascriptContext(_Context.ViewModelUpdater);
        }
    }
}
