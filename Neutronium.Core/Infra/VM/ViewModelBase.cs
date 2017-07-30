using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Neutronium.Core.Infra.VM
{
    public abstract class NotifyPropertyChangedBase :  INotifyPropertyChanged
    {
        protected bool Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(property, value))
                return false;

            property = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
