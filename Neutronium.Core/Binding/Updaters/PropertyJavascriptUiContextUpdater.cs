using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.Updaters
{
    internal class PropertyJavascriptUiContextUpdater : IJavascriptUIContextUpdater
    {
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private readonly object _Sender;
        private readonly string _PropertyName;

        internal PropertyJavascriptUiContextUpdater(IJsUpdateHelper jsUpdateHelper, object sender, string propertyName)
        {
            _JsUpdateHelper = jsUpdateHelper;
            _Sender = sender;
            _PropertyName = propertyName;
        }

        public IJavascriptJsContextUpdater ExecuteOnUiContext(ObjectChangesListener off)
        {
            var currentFather = _JsUpdateHelper.GetCached<JsGenericObject>(_Sender);
            if (currentFather == null)
                return null;

            var propertyUpdater = currentFather.GetPropertyUpdater(_PropertyName);
            if (!propertyUpdater.IsValid)
                return null;

            var newValue = propertyUpdater.GetCurrentChildValue();
            if (!propertyUpdater.HasChanged(newValue))
                return null;

            var newJsValue = _JsUpdateHelper.Map(newValue);
            if (newJsValue == null)
                return null;

            var bridgeUpdater = currentFather.GetUpdaterChangeOnCSharpContext(propertyUpdater, newJsValue);
            _JsUpdateHelper.UpdateOnUiContext(bridgeUpdater, off);
            return new PropertyJsContextUpdater(_JsUpdateHelper, bridgeUpdater, newJsValue);
        }

        private class PropertyJsContextUpdater : IJavascriptJsContextUpdater
        {
            private readonly IJsUpdateHelper _JsUpdateHelper;
            private readonly BridgeUpdater _BridgeUpdater;
            private readonly IJsCsGlue _NewJsValue;

            public PropertyJsContextUpdater(IJsUpdateHelper jsUpdateHelper, BridgeUpdater bridgeUpdater, IJsCsGlue newJsValue)
            {
                _BridgeUpdater = bridgeUpdater;
                _NewJsValue = newJsValue;
                _JsUpdateHelper = jsUpdateHelper;
            }

            public void ExecuteOnJsContext() => _JsUpdateHelper.UpdateOnJavascriptContext(_BridgeUpdater, _NewJsValue);
        }
    }
}
