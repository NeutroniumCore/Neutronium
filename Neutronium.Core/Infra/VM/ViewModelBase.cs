using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Neutronium.Core.Infra.VM
{
    public abstract class NotifyPropertyChangedBase :  INotifyPropertyChanged
    {
        protected bool Set<T>(ref T ipnv, T value, [CallerMemberName] string ipn=null)
        {
            if (object.Equals(ipnv, value))
                return false;

            ipnv = value;
            OnPropertyChanged(ipn);
            return true;
        }

        protected virtual void OnPropertyChanged(string pn)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pn));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
