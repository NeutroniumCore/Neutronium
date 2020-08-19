using System;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using System.Collections.Specialized;
using System.Linq;

namespace Neutronium.Core.Binding.Updaters
{
    internal class CollectionJavascriptUiContextUpdater : IJavascriptUIContextUpdater
    {
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private readonly object _Sender;
        private readonly NotifyCollectionChangedEventArgs _Change;

        public CollectionJavascriptUiContextUpdater(IJsUpdateHelper jsUpdateHelper, object sender, NotifyCollectionChangedEventArgs change)
        {
            _JsUpdateHelper = jsUpdateHelper;
            _Sender = sender;
            _Change = change;
        }

        private Tuple<BridgeUpdater, List<IJsCsGlue>> GetBridgeUpdater(JsArray array)
        {
            switch (_Change.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    return GetUpdater(array.GetAddUpdater, array.GetAddUpdater);

                case NotifyCollectionChangedAction.Replace:
                    return GetUpdater(array.GetReplaceUpdater, array.GetReplaceUpdater);

                case NotifyCollectionChangedAction.Remove:
                    return FromBridge(array.GetRemoveUpdater(_Change.OldStartingIndex, _Change.OldItems.Count));

                case NotifyCollectionChangedAction.Reset:
                    return FromBridge(array.GetResetUpdater());

                case NotifyCollectionChangedAction.Move:
                    return FromBridge(array.GetMoveUpdater(_Change.OldStartingIndex, _Change.NewStartingIndex));

                default:
                    return null;
            }
        }

        private static Tuple<BridgeUpdater, List<IJsCsGlue>> FromBridge(BridgeUpdater bridgeUpdater) => Tuple.Create<BridgeUpdater, List<IJsCsGlue>>(bridgeUpdater, null);

        private Tuple<BridgeUpdater, List<IJsCsGlue>> GetUpdater(Func<IJsCsGlue, int, BridgeUpdater> oneChangeBridgeCreator, 
            Func<List<IJsCsGlue>, int, BridgeUpdater> manyChangesBridgeCreator)
        {
            var newValues = InitValuesFromNewItems();
            var bridgeUpdater = _Change.NewItems.Count == 1
                ? oneChangeBridgeCreator(newValues[0], _Change.NewStartingIndex)
                : manyChangesBridgeCreator(newValues, _Change.NewStartingIndex);

            return Tuple.Create(bridgeUpdater, newValues);
        }

        private List<IJsCsGlue> InitValuesFromNewItems() =>  _Change.NewItems.Cast<object>().Select(item => _JsUpdateHelper.Map(item)).ToList();

        public IJavascriptJsContextUpdater ExecuteOnUiContext(ObjectChangesListener off)
        {
            var array = _JsUpdateHelper.GetCached<JsArray>(_Sender);
            if (array == null)
                return null;

            var updater = GetBridgeUpdater(array);
            _JsUpdateHelper.UpdateOnUiContext(updater.Item1, off);
            return new CollectionsContextUpdater(_JsUpdateHelper, updater.Item1, updater.Item2);
        }

        private class CollectionsContextUpdater : IJavascriptJsContextUpdater
        {
            private readonly IJsUpdateHelper _JsUpdateHelper;
            private readonly BridgeUpdater _BridgeUpdater;
            private readonly List<IJsCsGlue> _NewJsValues;

            public CollectionsContextUpdater(IJsUpdateHelper jsUpdateHelper, BridgeUpdater bridgeUpdater, List<IJsCsGlue> newJsValues)
            {
                _BridgeUpdater = bridgeUpdater;
                _NewJsValues = newJsValues;
                _JsUpdateHelper = jsUpdateHelper;
            }

            public void ExecuteOnJsContext() => _JsUpdateHelper.UpdateOnJavascriptContext(_BridgeUpdater, _NewJsValues);
        }
    }
}