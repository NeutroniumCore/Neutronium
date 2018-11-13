using System.Collections.Specialized;
using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface IJsUpdater 
    {
        T GetCached<T>(object value) where T : class, IJsCsGlue;
        IJsCsGlue Map(object value);
        BridgeUpdater UpdateBridgeUpdater(BridgeUpdater value);
        void UpdateOnJavascriptContextAllContext(BridgeUpdater updater, IJsCsGlue value);
        IJavascriptUpdater GetUpdaterForPropertyChanged(object sender, PropertyChangedEventArgs e);
        IJavascriptUpdater GetUpdaterForNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
    }
}