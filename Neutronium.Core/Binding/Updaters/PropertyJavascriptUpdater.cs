using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Updaters 
{
    internal class PropertyJavascriptUpdater: IJavascriptUpdater
    {
        private readonly IJsUpdater _JsUpdater;
        private readonly object _Sender;
        private readonly string _PropertyName;
        private object _NewValue;
        private BridgeUpdater _BridgeUpdater;
        private IJsCsGlue _NewJsValue;

        public bool NeedToRunOnJsContext => _BridgeUpdater != null;

        internal PropertyJavascriptUpdater(IJsUpdater updater, object sender, PropertyChangedEventArgs e) 
        {
            _JsUpdater = updater;
            _Sender = sender;
            _PropertyName = e.PropertyName;
        }

        public void OnUiContext() 
        {
            _JsUpdater.CheckUiContext();

            var currentfather = _JsUpdater.GetCached(_Sender);
            if (currentfather == null)
                return;

            var propertyUpdater = currentfather.GetPropertyUpdater(_PropertyName);
            if (!propertyUpdater.IsValid)
                return;

            _NewValue = propertyUpdater.GetCurrentChildValue();
            if (!propertyUpdater.HasChanged(_NewValue))
                return;
           
            //if (!_IsLoaded) {
            //    //Changes happening on the UI thread while javascript thread is still initializing bindings
            //    //We keep the updates here to be replayed when binding is done
            //    _UpdatesToBeReplayed = _UpdatesToBeReplayed ?? new List<Action>();
            //    _UpdatesToBeReplayed.Add(() => UpdateFromCSharpChanges(_NewValue, updaterBuilder));
            //    return;
            //}

            _NewJsValue = _JsUpdater.Map(_NewValue);
            if (_NewJsValue == null)
                return;

            _BridgeUpdater = _JsUpdater.UpdateBridgeUpdater(currentfather.GetUpdater(propertyUpdater, _NewJsValue));
        }

        public void OnJsContext() 
        {
            _JsUpdater.UpdateOnJavascriptContextAllContext(_BridgeUpdater, _NewJsValue);
        }
    }
}
