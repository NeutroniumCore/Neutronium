using MVVM.HTML.Core.HTMLBinding;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MVVM.HTML.Core.Binding.Listeners
{
    public class ListenableVisitor : IListenableObjectVisitor
    {
        private readonly Action<INotifyPropertyChanged> _OnObject;
        private readonly Action<INotifyCollectionChanged> _OnCollection;
        private readonly Action<JSCommand> _OnCommand;
        public ListenableVisitor(Action<INotifyPropertyChanged> iOnObject,
                            Action<INotifyCollectionChanged> iOnCollection, Action<JSCommand> iOnCommand)
        {
            _OnObject = iOnObject;
            _OnCollection = iOnCollection;
            _OnCommand = iOnCommand;
        }

        public void OnObject(INotifyPropertyChanged iobject)
        {
            _OnObject(iobject);
        }

        public void OnCollection(INotifyCollectionChanged icollection)
        {
            _OnCollection(icollection);
        }

        public void OnCommand(JSCommand icommand)
        {
            _OnCommand(icommand);
        }
    }
}
