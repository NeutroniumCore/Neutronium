using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using MoreCollection.Extensions;

namespace Neutronium.Core.Binding.Updaters
{
    internal class JsUpdateHelper : IJsUpdateHelper, IJsUpdaterFactory
    {
        private readonly HtmlViewContext _Context;
        private readonly Lazy<IJavascriptObjectBuilderStrategy> _BuilderStrategy;
        private readonly ISessionMapper _SessionMapper;
        private readonly SessionCacher _SessionCache;

        public event EventHandler<EventArgs> OnJavascriptSessionReady;

        internal CSharpToJavascriptConverter JsObjectBuilder { get; set; }

        public bool isInUiContext => _Context.UiDispatcher.IsInContext();

        internal JsUpdateHelper(ISessionMapper sessionMapper, HtmlViewContext context, Func<IJavascriptObjectBuilderStrategy> strategy, SessionCacher sessionCache)
        {
            _SessionMapper = sessionMapper;
            _Context = context;
            _BuilderStrategy = new Lazy<IJavascriptObjectBuilderStrategy>(strategy);
            _SessionCache = sessionCache;
            _SessionMapper.OnJavascriptSessionReady += _SessionMapper_OnJavascriptSessionReady;
        }

        private void _SessionMapper_OnJavascriptSessionReady(object sender, EventArgs e)
        {
            OnJavascriptSessionReady?.Invoke(this, e);
        }

        public IJavascriptUIContextUpdater GetUpdaterForPropertyChanged(object sender, string propertyName)
        {
            return new PropertyJavascriptUiContextUpdater(this, sender, propertyName);
        }

        public IJavascriptUIContextUpdater GetUpdaterForNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            return new CollectionJavascriptUiContextUpdater(this, sender, e);
        }

        public IJavascriptUIContextUpdater GetUpdaterForExecutionChanged(object sender)
        {
            return new CommandJavascriptUiContextUpdater(this, sender);
        }

        public void CheckUiContext()
        {
            if (_Context.UiDispatcher.IsInContext())
                return;

            throw ExceptionHelper.Get("MVVM ViewModel should be updated from UI thread. Use await pattern and Dispatcher to do so.");
        }

        public void DispatchInJavascriptContext(Action action)
        {
            _Context.WebView.Dispatch(action);
        }

        public void DispatchInUiContext(Action action)
        {
            _Context.UiDispatcher.Dispatch(action);
        }

        public void DispatchInUiContextBindingPriority(Action action)
        {
            _Context.UiDispatcher.DispatchWithBindingPriority(action);
        }

        public T GetCached<T>(object value) where T : class
        {
            return _SessionCache.GetCached(value) as T;
        }

        public IJsCsGlue Map(object value)
        {
            return JsObjectBuilder.Map(value);
        }

        public void UpdateOnUiContext(BridgeUpdater updater, ObjectChangesListener off)
        {
            updater.CleanAfterChangesOnUiThread(off, _SessionCache);
        }

        public void UpdateOnJavascriptContext(BridgeUpdater updater, IList<IJsCsGlue> values)
        {
            if (values == null || values.Count == 0)
            {
                UpdateOnJavascriptContext(updater);
                return;
            }

            if (_Context.JavascriptFrameworkIsMappingObject)
            {
                UpdateOnJavascriptContextWithMapping(updater, values).DoNotWait();
                return;
            }

            UpdateOnJavascriptContextWithoutMapping(updater, values);
        }

        private void UpdateOnJavascriptContextWithoutMapping(BridgeUpdater updater, IEnumerable<IJsCsGlue> values)
        {
            values.ForEach(UpdateJavascriptValue);
            UpdateOnJavascriptContext(updater);
        }

        private async Task UpdateOnJavascriptContextWithMapping(BridgeUpdater updater, IList<IJsCsGlue> values)
        {
            values.ForEach(UpdateJavascriptValue);
            foreach (var jsCsGlue in values)
            {
                await _SessionMapper.InjectInHtmlSession(jsCsGlue);
            }    
            UpdateOnJavascriptContext(updater);
        }

        public void UpdateOnJavascriptContext(BridgeUpdater updater, IJsCsGlue value)
        {
            if (value == null)
            {
                UpdateOnJavascriptContext(updater);
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
            UpdateJavascriptValue(value);
            UpdateOnJavascriptContext(updater);
        }

        private async Task UpdateOnJavascriptContextWithMapping(BridgeUpdater updater, IJsCsGlue value)
        {
            UpdateJavascriptValue(value);
            await _SessionMapper.InjectInHtmlSession(value);
            UpdateOnJavascriptContext(updater);
        }

        private void UpdateJavascriptValue(IJsCsGlue value)
        {
            _BuilderStrategy.Value.UpdateJavascriptValue(value);
        }

        public void UpdateOnJavascriptContext(BridgeUpdater updater)
        {
            updater.UpdateOnJavascriptContext(_Context.ViewModelUpdater, _SessionCache);
        }
    }
}
