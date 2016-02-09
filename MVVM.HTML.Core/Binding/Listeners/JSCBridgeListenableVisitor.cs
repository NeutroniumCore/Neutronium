using MVVM.HTML.Core.HTMLBinding;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MVVM.HTML.Core.Binding.Listeners
{
    public class JSCBridgeListenableVisitor : IListenableObjectVisitor
    {
        private Action<INotifyPropertyChanged> _OnObject;
        private Action<INotifyCollectionChanged> _OnCollection;
        private Action<JSCommand> _OnCommand;
        public JSCBridgeListenableVisitor(Action<INotifyPropertyChanged> iOnObject,
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
