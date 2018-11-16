using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.Listeners
{
    public interface IObjectChangesListener
    {
        void OnObject(INotifyPropertyChanged @object);

        void OnCollection(INotifyCollectionChanged collection);

        void OnCommand(ICommand command);

        void OnCommand(IUpdatableCommand command);
    }
}
