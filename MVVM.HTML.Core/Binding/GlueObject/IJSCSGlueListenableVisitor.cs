using System.Collections.Specialized;
using System.ComponentModel;

namespace MVVM.HTML.Core.HTMLBinding
{
    public interface IJSCSGlueListenableVisitor
    {
        void OnObject(INotifyPropertyChanged iobject);

        void OnCollection(INotifyCollectionChanged icollection);

        void OnCommand(JSCommand icommand);
    }
}
