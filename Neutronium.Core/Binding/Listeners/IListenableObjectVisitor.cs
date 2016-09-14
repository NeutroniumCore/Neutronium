using System.Collections.Specialized;
using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Listeners
{
    public interface IListenableObjectVisitor
    {
        void OnObject(INotifyPropertyChanged iobject);

        void OnCollection(INotifyCollectionChanged icollection);

        void OnCommand(JSCommand icommand);
    }
}
