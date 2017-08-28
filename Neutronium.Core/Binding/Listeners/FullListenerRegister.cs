using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Listeners
{
    internal class FullListenerRegister
    {
        public IEntityUpdater<INotifyPropertyChanged> Property { get; }
        public IEntityUpdater<INotifyCollectionChanged> Collection { get; }
        public IEntityUpdater<JsCommand> Command { get; }
        public ObjectChangesListener On { get; }
        public ObjectChangesListener Off { get; }

        public FullListenerRegister(Action<INotifyPropertyChanged> propertyOn, Action<INotifyPropertyChanged> propertyOff,
                        Action<INotifyCollectionChanged> collectionOn, Action<INotifyCollectionChanged> collectionOff,
                        Action<JsCommand> jsCommandOn, Action<JsCommand> jsCommandOff)
        {
            Property = new ListenerRegister<INotifyPropertyChanged>(propertyOn, propertyOff);
            Collection = new ListenerRegister<INotifyCollectionChanged>(collectionOn, collectionOff);
            Command = new ListenerRegister<JsCommand>(jsCommandOn, jsCommandOff);
            On = new ObjectChangesListener(Property.OnEnter, Collection.OnEnter, Command.OnEnter);
            Off = new ObjectChangesListener(Property.OnExit, Collection.OnExit, Command.OnExit);
        }

        public Silenter<INotifyCollectionChanged> GetColllectionSilenter(object target)
        {
            return Silenter.GetSilenter(Collection, target);
        }

        public Silenter<INotifyPropertyChanged> GetPropertySilenter(object target)
        {
            return Silenter.GetSilenter(Property, target);
        }
    }
}
