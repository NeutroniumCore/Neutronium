using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject.Executable;

namespace Neutronium.Core.Binding.Listeners
{
    internal class FullListenerRegister
    {
        public ObjectChangesListener On { get; }
        public ObjectChangesListener Off { get; }

        private IEntityUpdater<INotifyPropertyChanged> Property { get; }
        private IEntityUpdater<INotifyCollectionChanged> Collection { get; }
        private IEntityUpdater<JsCommandBase> Command { get; }

        public FullListenerRegister(PropertyChangedEventHandler propertyHandler,
            NotifyCollectionChangedEventHandler collectionHandler) :
            this(n => n.PropertyChanged += propertyHandler, n => n.PropertyChanged -= propertyHandler,
                n => n.CollectionChanged += collectionHandler, n => n.CollectionChanged -= collectionHandler,
                c => c.ListenChanges(), c => c.UnListenChanges())
        {        
        }

        private FullListenerRegister(Action<INotifyPropertyChanged> propertyOn, Action<INotifyPropertyChanged> propertyOff,
                        Action<INotifyCollectionChanged> collectionOn, Action<INotifyCollectionChanged> collectionOff,
                        Action<JsCommandBase> jsCommandOn, Action<JsCommandBase> jsCommandOff)
        {
            Property = new ListenerRegister<INotifyPropertyChanged>(propertyOn, propertyOff);
            Collection = new ListenerRegister<INotifyCollectionChanged>(collectionOn, collectionOff);
            Command = new ListenerRegister<JsCommandBase>(jsCommandOn, jsCommandOff);
            On = new ObjectChangesListener(Property.OnEnter, Collection.OnEnter, Command.OnEnter);
            Off = new ObjectChangesListener(Property.OnExit, Collection.OnExit, Command.OnExit);
        }

        public Silenter<INotifyCollectionChanged> GetColllectionSilenter(object target)
        {
            return Silenter.GetSilenter(Collection, target);
        }

        public PropertyChangedSilenter GetPropertySilenter(object target, string propertyName)
        {
            return Silenter.GetSilenter(Property, target, propertyName);
        }
    }
}
