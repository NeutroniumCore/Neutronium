using System.Collections.Specialized;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Updaters
{
    internal class CollectionJavascriptUpdater : IJavascriptUpdater
    {
        private readonly IJsUpdater _JsUpdater;
        private readonly object _Sender;
        private readonly NotifyCollectionChangedEventArgs _Change;
        private BridgeUpdater _BridgeUpdater;
        private IJsCsGlue _NewJsValue;

        public bool NeedToRunOnJsContext => _BridgeUpdater?.HasUpdatesOnJavascriptContext == true;

        public CollectionJavascriptUpdater(IJsUpdater jsUpdater, object sender, NotifyCollectionChangedEventArgs change)
        {
            _JsUpdater = jsUpdater;
            _Sender = sender;
            _Change = change;
        }

        public void OnUiContext()
        {
            var array = _JsUpdater.GetCached<JsArray>(_Sender);
            if (array == null)
                return;

            var updater = GetBridgeUpdater(array);
            if (updater == null)
                return;

            _BridgeUpdater = _JsUpdater.UpdateBridgeUpdater(updater);
        }

        private BridgeUpdater GetBridgeUpdater(JsArray array)
        {
            switch (_Change.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _NewJsValue = _JsUpdater.Map(_Change.NewItems[0]);
                    if (_NewJsValue == null)
                        return null;
                    return array.GetAddUpdater(_NewJsValue, _Change.NewStartingIndex);

                case NotifyCollectionChangedAction.Replace:
                    _NewJsValue = _JsUpdater.Map(_Change.NewItems[0]);
                    if (_NewJsValue == null)
                        return null;
                    return array.GetReplaceUpdater(_NewJsValue, _Change.NewStartingIndex);

                case NotifyCollectionChangedAction.Remove:
                    return array.GetRemoveUpdater(_Change.OldStartingIndex);

                case NotifyCollectionChangedAction.Reset:
                    return array.GetResetUpdater();;

                case NotifyCollectionChangedAction.Move:
                    return array.GetMoveUpdater(_Change.OldStartingIndex, _Change.NewStartingIndex);

                default:
                    return null;
            }
        }

        public void OnJsContext()
        {
            _JsUpdater.UpdateOnJavascriptContextAllContext(_BridgeUpdater, _NewJsValue);
        }
    }
}