using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface IJsUpdateHelper 
    {
        T GetCached<T>(object value) where T : class, IJsCsGlue;
        IJsCsGlue Map(object value);
        void UpdateOnUiContext(BridgeUpdater updater, ObjectChangesListener off);
        void UpdateOnJavascriptContext(BridgeUpdater updater, IJsCsGlue value);

        IJavascriptUpdater GetUpdaterForPropertyChanged(object sender, PropertyChangedEventArgs e);
        IJavascriptUpdater GetUpdaterForNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);

        void CheckUiContext();
        void DispatchInJavascriptContext(Action action);
    }
}