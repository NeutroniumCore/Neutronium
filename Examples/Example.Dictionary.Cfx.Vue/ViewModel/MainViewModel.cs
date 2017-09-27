using System.Dynamic;
using System.Windows.Input;
using Neutronium.Example.ViewModel.Infra;

namespace Example.Dictionary.Cfx.Vue.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        public MainViewModel(ExpandoObject child)
        {
            Dynamic = child;
            ChangeAttribute = new RelayCommand(DoChangeAttribute);
            AddAttribute = new RelayCommand(DoAddAttribute);
        }

        private void DoChangeAttribute()
        {
            dynamic alterableChild = Dynamic;
            int currentValue = alterableChild.Value;
            alterableChild.Value = currentValue + 1;
        }

        private void DoAddAttribute()
        {
            dynamic alterableChild = Dynamic;
            alterableChild.NewAttribute = "Hello";
        }

        public ExpandoObject Dynamic { get; }
        public ICommand ChangeAttribute { get; }
        public ICommand AddAttribute { get; }
    }
}
