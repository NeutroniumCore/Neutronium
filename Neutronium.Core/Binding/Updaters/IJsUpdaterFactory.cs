using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface IJsUpdaterFactory: IContextsManager
    {
        IJavascriptUpdater GetUpdaterForPropertyChanged(object sender, string propertyName);
        IJavascriptUpdater GetUpdaterForNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
        IJavascriptUpdater GetUpdaterForExecutionChanged(object sender);
        event EventHandler<EventArgs> OnJavascriptSessionReady;
    }
}
