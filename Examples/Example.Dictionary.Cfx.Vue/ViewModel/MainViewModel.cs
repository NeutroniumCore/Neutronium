using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Input;
using Neutronium.Example.ViewModel.Infra;

namespace Example.Dictionary.Cfx.Vue.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        private readonly Random _Random = new Random();

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
            var alterableChild = Dynamic as IDictionary<string, object>;
            alterableChild[$"Attribute{_Random.Next(10000)}"] = _Random.Next(10000);
        }

        public ExpandoObject Dynamic { get; }
        public ICommand ChangeAttribute { get; }
        public ICommand AddAttribute { get; }
    }
}
