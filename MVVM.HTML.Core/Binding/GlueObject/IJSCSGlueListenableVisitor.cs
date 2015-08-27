using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MVVM.HTML.Core.HTMLBinding
{
    public interface IJSCSGlueListenableVisitor
    {
        void OnObject(INotifyPropertyChanged iobject);

        void OnCollection(INotifyCollectionChanged icollection);

        void OnCommand(JSCommand icommand);
    }
}
