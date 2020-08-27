using System.Collections.Specialized;

namespace Neutronium.Core.Binding.Listeners
{
    internal interface ICSharpChangesListener
    {
        ObjectChangesListener On { get; }
        ObjectChangesListener Off { get; }

        Silenter<INotifyCollectionChanged> GetCollectionSilenter(object target);
        PropertyChangedSilenter GetPropertySilenter(object target, string propertyName);
        void ReportPropertyChanged(object sender, string propertyName);
    }
}
