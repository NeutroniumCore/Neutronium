using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class ViewModelTestBase : INotifyPropertyChanged
    {
        internal int ListenerCount => _PropertyChanged?.GetInvocationList().Length ?? 0;
        private event PropertyChangedEventHandler _PropertyChanged;

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
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _PropertyChanged += value; }
            remove { _PropertyChanged -= value; }
        }
    }
}
