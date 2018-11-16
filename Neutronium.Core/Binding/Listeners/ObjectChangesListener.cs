using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.Listeners
{
    public class ObjectChangesListener : IObjectChangesListener
    {
        private readonly Action<INotifyPropertyChanged> _OnObject;
        private readonly Action<INotifyCollectionChanged> _OnCollection;
        private readonly Action<ICommand> _OnCommand;
        private readonly Action<IUpdatableCommand> _OnUpdatableCommand;

        public ObjectChangesListener(Action<INotifyPropertyChanged> onObject,
                            Action<INotifyCollectionChanged> onCollection, 
                            Action<ICommand> onCommand,
                            Action<IUpdatableCommand> onUpdatableCommand)
        {
            _OnObject = onObject;
            _OnCollection = onCollection;
            _OnCommand = onCommand;
            _OnUpdatableCommand = onUpdatableCommand;
        }

        public void OnObject(INotifyPropertyChanged listenedObject)
        {
            _OnObject(listenedObject);
        }

        public void OnCollection(INotifyCollectionChanged collection)
        {
            _OnCollection(collection);
        }

        public void OnCommand(ICommand command)
        {
            _OnCommand(command);
        }

        public void OnCommand(IUpdatableCommand command)
        {
            _OnUpdatableCommand(command);
        }      
    }
}
