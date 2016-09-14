using System.ComponentModel;

namespace Neutronium.Example.ViewModel.Infra
{
    public class ViewModelBase : INotifyPropertyChanged
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pn));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
