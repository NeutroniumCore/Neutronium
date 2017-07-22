using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Listeners
{
    public class ObjectChangesListener : IObjectChangesListener
    {
        private readonly Action<INotifyPropertyChanged> _OnObject;
        private readonly Action<INotifyCollectionChanged> _OnCollection;
        private readonly Action<JSCommand> _OnCommand;

        public ObjectChangesListener(Action<INotifyPropertyChanged> onObject,
                            Action<INotifyCollectionChanged> onCollection, Action<JSCommand> onCommand)
        {
            _OnObject = onObject;
            _OnCollection = onCollection;
            _OnCommand = onCommand;
        }

        public void OnObject(INotifyPropertyChanged listenedObject)
        {
            _OnObject(listenedObject);
        }

        public void OnCollection(INotifyCollectionChanged collection)
        {
            _OnCollection(collection);
        }

        public void OnCommand(JSCommand command)
        {
            _OnCommand(command);
        }
    }
}
