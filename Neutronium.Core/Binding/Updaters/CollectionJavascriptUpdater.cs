using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using System.Collections.Specialized;

namespace Neutronium.Core.Binding.Updaters
{
    internal class CollectionJavascriptUpdater : IJavascriptUpdater
    {
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private readonly object _Sender;
        private readonly NotifyCollectionChangedEventArgs _Change;
        private BridgeUpdater _BridgeUpdater;
        private List<IJsCsGlue> _NewJsValues;

        private List<IJsCsGlue> NewJsValues => _NewJsValues ?? (_NewJsValues = new List<IJsCsGlue>());
        public bool NeedToRunOnJsContext => _BridgeUpdater?.HasUpdatesOnJavascriptContext == true;

        public CollectionJavascriptUpdater(IJsUpdateHelper jsUpdateHelper, object sender, NotifyCollectionChangedEventArgs change)
        {
            _JsUpdateHelper = jsUpdateHelper;
            _Sender = sender;
            _Change = change;
        }

        public void OnUiContext(ObjectChangesListener off)
        {
            var array = _JsUpdateHelper.GetCached<JsArray>(_Sender);
            if (array == null)
                return;

            _BridgeUpdater = GetBridgeUpdater(array);
            _JsUpdateHelper.UpdateOnUiContext(_BridgeUpdater, off);
        }

        private BridgeUpdater GetBridgeUpdater(JsArray array)
        {
            var newValue = default(IJsCsGlue);
            switch (_Change.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    newValue = _JsUpdateHelper.Map(_Change.NewItems[0]);
                    if (newValue == null)
                        return null;
                    NewJsValues.Add(newValue);
                    return array.GetAddUpdater(newValue, _Change.NewStartingIndex);

                case NotifyCollectionChangedAction.Replace:
                    newValue = _JsUpdateHelper.Map(_Change.NewItems[0]);
                    if (newValue == null)
                        return null;
                    NewJsValues.Add(newValue);
                    return array.GetReplaceUpdater(newValue, _Change.NewStartingIndex);

                case NotifyCollectionChangedAction.Remove:
                    return array.GetRemoveUpdater(_Change.OldStartingIndex);

                case NotifyCollectionChangedAction.Reset:
                    return array.GetResetUpdater(); ;

                case NotifyCollectionChangedAction.Move:
                    return array.GetMoveUpdater(_Change.OldStartingIndex, _Change.NewStartingIndex);

                default:
                    return null;
            }
        }

        public void OnJsContext()
        {
            _JsUpdateHelper.UpdateOnJavascriptContext(_BridgeUpdater, _NewJsValues);
        }
    }
}