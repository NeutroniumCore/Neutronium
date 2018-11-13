using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Updaters 
{
    internal class PropertyJavascriptUpdater: IJavascriptUpdater
    {
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private readonly object _Sender;
        private readonly string _PropertyName;
        private object _NewValue;
        private BridgeUpdater _BridgeUpdater;
        private IJsCsGlue _NewJsValue;

        public bool NeedToRunOnJsContext => _BridgeUpdater != null;

        internal PropertyJavascriptUpdater(IJsUpdateHelper jsUpdateHelper, object sender, PropertyChangedEventArgs e) 
        {
            _JsUpdateHelper = jsUpdateHelper;
            _Sender = sender;
            _PropertyName = e.PropertyName;
        }

        public void OnUiContext() 
        {
            var currentfather = _JsUpdateHelper.GetCached<JsGenericObject>(_Sender);
            if (currentfather == null)
                return;

            var propertyUpdater = currentfather.GetPropertyUpdater(_PropertyName);
            if (!propertyUpdater.IsValid)
                return;

            _NewValue = propertyUpdater.GetCurrentChildValue();
            if (!propertyUpdater.HasChanged(_NewValue))
                return;

            _NewJsValue = _JsUpdateHelper.Map(_NewValue);
            if (_NewJsValue == null)
                return;

            _BridgeUpdater = _JsUpdateHelper.UpdateBridgeUpdater(currentfather.GetUpdater(propertyUpdater, _NewJsValue));
        }

        public void OnJsContext() 
        {
            _JsUpdateHelper.UpdateOnJavascriptContextAllContext(_BridgeUpdater, _NewJsValue);
        }
    }
}
