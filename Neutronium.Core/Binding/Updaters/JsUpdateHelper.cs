using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Exceptions;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Neutronium.Core.Infra;

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

        public IJavascriptUpdater GetUpdaterForPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            return new PropertyJavascriptUpdater(this, sender, e);
        }

        public IJavascriptUpdater GetUpdaterForNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            return new CollectionJavascriptUpdater(this, sender, e);
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

        public T GetCached<T>(object value) where T : class, IJsCsGlue
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


        public void UpdateOnJavascriptContext(BridgeUpdater updater, IJsCsGlue value)
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
            UpdateJavascriptValue(value);
            RunUpdaterOnJsContext(updater);
        }

        private async Task UpdateOnJavascriptContextWithMapping(BridgeUpdater updater, IJsCsGlue value)
        {
            UpdateJavascriptValue(value);
            await _SessionMapper.InjectInHtmlSession(value);
            RunUpdaterOnJsContext(updater);
        }

        private void UpdateJavascriptValue(IJsCsGlue value) 
        {
            _BuilderStrategy.Value.UpdateJavascriptValue(value);
        }

        private void RunUpdaterOnJsContext(BridgeUpdater updater)
        {
            updater.UpdateOnJavascriptContext(_Context.ViewModelUpdater);
        }
    }
}
