using MVVM.HTML.Core.HTMLBinding;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MVVM.HTML.Core.Binding.Listeners
{
    public interface IListenableObjectVisitor
    {
        void OnObject(INotifyPropertyChanged iobject);

        void OnCollection(INotifyCollectionChanged icollection);

        void OnCommand(JSCommand icommand);
    }
}
