using System;
using System.Collections.Specialized;

namespace Neutronium.Core.Binding.Updater
{
    internal interface IJsUpdaterFactory: IContextsManager
    {
        IJavascriptUIContextUpdater GetUpdaterForPropertyChanged(object sender, string propertyName);
        IJavascriptUIContextUpdater GetUpdaterForNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
        IJavascriptUIContextUpdater GetUpdaterForExecutionChanged(object sender);
        event EventHandler<EventArgs> OnJavascriptSessionReady;
    }
}
