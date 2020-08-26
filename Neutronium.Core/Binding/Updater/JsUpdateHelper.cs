using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.JavascriptFrameworkMapper;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Updater
{
    internal class JsUpdateHelper : IJsUpdateHelper, IJsUpdaterFactory
    {
        private readonly HtmlViewContext _Context;
        private readonly Lazy<IJavascriptFrameworkMapper> _JavascriptFrameworkMapper;
        private readonly ISessionCache _SessionCache;

        public event EventHandler<EventArgs> OnJavascriptSessionReady;

        internal ICSharpToGlueMapper GlueMapper { get; set; }

        public bool IsInUiContext => _Context.UiDispatcher.IsInContext();
        public IWebSessionLogger Logger => _Context.Logger;
        private IJavascriptFrameworkMapper JavascriptFrameworkMapper => _JavascriptFrameworkMapper.Value;

        internal JsUpdateHelper(IBindingLifeCycle bindingLifeCycle, HtmlViewContext context, Func<IJavascriptFrameworkMapper> frameworkMapper, ISessionCache sessionCache)
        {
            _Context = context;
            _JavascriptFrameworkMapper = new Lazy<IJavascriptFrameworkMapper>(frameworkMapper);
            _SessionCache = sessionCache;
            bindingLifeCycle.OnJavascriptSessionReady += BindingLifeCycleOnJavascriptSessionReady;
        }

        private void BindingLifeCycleOnJavascriptSessionReady(object sender, EventArgs e)
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
            _Context.CheckUiContext();
        }

        public void DispatchInJavascriptContext(Action action) => _Context.WebView.Dispatch(action);

        public void DispatchInUiContext(Action action) => _Context.UiDispatcher.Dispatch(action);

        public void DispatchInUiContextBindingPriority(Action action) => _Context.UiDispatcher.DispatchWithBindingPriority(action);

        public Task<T> EvaluateOnUiContextAsync<T>(Func<T> compute) => _Context.EvaluateOnUiContextAsync(compute);

        public Task RunOnJavascriptContextAsync(Action execute) => _Context.RunOnJavascriptContextAsync(execute);

        public T GetCached<T>(object value) where T : class
        {
            return _SessionCache.GetCached(value) as T;
        }

        public T GetCachedFromJsObject<T>(IJavascriptObject value) where T : class
        {
            return _SessionCache.GetCached(value) as T;
        }

        public IJsCsGlue Map(object value) => GlueMapper.Map(value);

        public bool GetSimpleValue(IJavascriptObject value, out object targetValue, Type targetType = null)
        {
            return _Context.WebView.Converter.GetSimpleValue(value, out targetValue, targetType);
        }

        public void UpdateOnUiContext(BridgeUpdater updater, ObjectChangesListener off)
        {
            updater?.CleanAfterChangesOnUiThread(off, _SessionCache);
        }

        public void UpdateOnJavascriptContext(BridgeUpdater updater, IList<IJsCsGlue> values) => JavascriptFrameworkMapper.UpdateOnJavascriptContext(updater, values);

        public void UpdateOnJavascriptContext(BridgeUpdater updater, IJsCsGlue value) => JavascriptFrameworkMapper.UpdateOnJavascriptContext(updater, value);

        public void UpdateOnJavascriptContext(BridgeUpdater updater) => JavascriptFrameworkMapper.UpdateOnJavascriptContext(updater);
    }
}
