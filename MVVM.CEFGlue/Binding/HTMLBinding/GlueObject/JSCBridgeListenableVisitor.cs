using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MVVM.CEFGlue.HTMLBinding
{
    public class JSCBridgeListenableVisitor : IJSCSGlueListenableVisitor
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
