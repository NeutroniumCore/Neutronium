using System.Collections.Specialized;
using System.ComponentModel;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface IJsUpdaterFactory: IContextsManager
    {
        IJavascriptUpdater GetUpdaterForPropertyChanged(object sender, PropertyChangedEventArgs e);
        IJavascriptUpdater GetUpdaterForNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
    }
}
