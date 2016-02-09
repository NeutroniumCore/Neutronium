using MVVM.HTML.Core.HTMLBinding;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MVVM.HTML.Core.Binding.Listeners
{
    internal class FullListenerRegister
    {

        public FullListenerRegister(Action<INotifyPropertyChanged> PropertyOn, Action<INotifyPropertyChanged> PropertyOff,
                        Action<INotifyCollectionChanged> CollectionOn, Action<INotifyCollectionChanged> CollectionOff,
                        Action<JSCommand> JSCommandOn, Action<JSCommand> JSCommandOff)
        {
            Property = new ListenerRegister<INotifyPropertyChanged>(PropertyOn, PropertyOff);
            Collection = new ListenerRegister<INotifyCollectionChanged>(CollectionOn, CollectionOff);
            Command = new ListenerRegister<JSCommand>(JSCommandOn, JSCommandOff);
        }

        public ListenerRegister<INotifyPropertyChanged> Property { get; private set; }

        public ListenerRegister<INotifyCollectionChanged> Collection { get; private set; }

        public ListenerRegister<JSCommand> Command { get; private set; }

        public ListenableVisitor GetOn()
        {
            return new ListenableVisitor(Property.On, Collection.On, Command.On);
        }

        public ListenableVisitor GetOff()
        {
            return new ListenableVisitor(Property.Off, Collection.Off, Command.Off);
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
