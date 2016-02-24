using System.Collections.Specialized;
using System.ComponentModel;
using MVVM.HTML.Core.Binding.GlueObject;

namespace MVVM.HTML.Core.Binding.Listeners
{
    public interface IListenableObjectVisitor
    {
        void OnObject(INotifyPropertyChanged iobject);

        void OnCollection(INotifyCollectionChanged icollection);

        void OnCommand(JSCommand icommand);
    }
}
