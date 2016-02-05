using System.ComponentModel;

namespace MVVM.ViewModel
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
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(pn));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
