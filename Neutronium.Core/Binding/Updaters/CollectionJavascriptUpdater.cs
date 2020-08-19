using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using System.Collections.Specialized;
using System.Linq;

namespace Neutronium.Core.Binding.Updaters
{
    internal class CollectionJavascriptUpdater : IJavascriptUpdater
    {
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private readonly object _Sender;
        private readonly NotifyCollectionChangedEventArgs _Change;
        private BridgeUpdater _BridgeUpdater;
        private List<IJsCsGlue> _NewJsValues;

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
            switch (_Change.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    return GetAddUpdater(array);

                case NotifyCollectionChangedAction.Replace:
                    return GetReplaceUpdater(array);

                case NotifyCollectionChangedAction.Remove:
                    return array.GetRemoveUpdater(_Change.OldStartingIndex, _Change.OldItems.Count);

                case NotifyCollectionChangedAction.Reset:
                    return array.GetResetUpdater();

                case NotifyCollectionChangedAction.Move:
                    return array.GetMoveUpdater(_Change.OldStartingIndex, _Change.NewStartingIndex);

                default:
                    return null;
            }
        }

        private BridgeUpdater GetAddUpdater(JsArray array)
        {
            InitValuesFromNewItems();
            if (_Change.NewItems.Count == 1)
            {
                return array.GetAddUpdater(_NewJsValues[0], _Change.NewStartingIndex);
            }
            return array.GetAddUpdater(_NewJsValues, _Change.NewStartingIndex);
        }

        private BridgeUpdater GetReplaceUpdater(JsArray array)
        {
            InitValuesFromNewItems();
            if (_Change.NewItems.Count == 1)
            {
                return array.GetReplaceUpdater(_NewJsValues[0], _Change.NewStartingIndex);
            }
            return array.GetReplaceUpdater(_NewJsValues, _Change.NewStartingIndex);
        }

        private void InitValuesFromNewItems()
        {
            _NewJsValues = _Change.NewItems.Cast<object>().Select(item => _JsUpdateHelper.Map(item))
                .ToList();
        }

        public void OnJsContext()
        {
            if (!NeedToRunOnJsContext)
                return;

            _JsUpdateHelper.UpdateOnJavascriptContext(_BridgeUpdater, _NewJsValues);
        }
    }
}