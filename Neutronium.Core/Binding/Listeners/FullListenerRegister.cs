using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Listeners
{
    internal class FullListenerRegister
    {
        public ListenerRegister<INotifyPropertyChanged> Property { get; }
        public ListenerRegister<INotifyCollectionChanged> Collection { get; }
        public ListenerRegister<JSCommand> Command { get; }
        public ListenableVisitor On { get; }
        public ListenableVisitor Off { get; }

        public FullListenerRegister(Action<INotifyPropertyChanged> propertyOn, Action<INotifyPropertyChanged> propertyOff,
                        Action<INotifyCollectionChanged> collectionOn, Action<INotifyCollectionChanged> collectionOff,
                        Action<JSCommand> jsCommandOn, Action<JSCommand> jsCommandOff)
        {
            Property = new ListenerRegister<INotifyPropertyChanged>(propertyOn, propertyOff);
            Collection = new ListenerRegister<INotifyCollectionChanged>(collectionOn, collectionOff);
            Command = new ListenerRegister<JSCommand>(jsCommandOn, jsCommandOff);
            On = new ListenableVisitor(Property.On, Collection.On, Command.On);
            Off = new ListenableVisitor(Property.Off, Collection.Off, Command.Off);
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
