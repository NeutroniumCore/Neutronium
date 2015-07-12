using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MVVM.CEFGlue.Infra.VM
{
    public abstract class NotifyPropertyChangedBase :  INotifyPropertyChanged
    {

        protected bool Set<T>(ref T ipnv, T value, string ipn)
        {
            if (object.Equals(ipnv, value))
                return false;

            ipnv = value;
            OnPropertyChanged(ipn);
            return true;
        }

        protected virtual void OnPropertyChanged(string pn)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(pn));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
