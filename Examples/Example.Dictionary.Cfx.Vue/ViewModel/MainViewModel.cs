using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Input;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.MVVMComponents.Relay;

namespace Example.Dictionary.Cfx.Vue.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        private readonly Random _Random = new Random();

        public MainViewModel(ExpandoObject child)
        {
            ExpandoObject = child;
            DynamicObject = new DynamicObjectViewModel();
            ChangeAttribute = new RelaySimpleCommand(DoChangeAttribute);
            AddAttribute = new RelaySimpleCommand(DoAddAttribute);
        }

        private void DoChangeAttribute()
        {
            dynamic alterableChild = ExpandoObject;
            int currentValue = alterableChild.Value;
            alterableChild.Value = currentValue + 1;
        }

        private void DoAddAttribute()
        {
            var alterableChild = ExpandoObject as IDictionary<string, object>;
            alterableChild[$"Attribute{_Random.Next(10000)}"] = _Random.Next(10000);
        }

        public ExpandoObject ExpandoObject { get; }
        public object DynamicObject { get; }
        public ICommand ChangeAttribute { get; }
        public ICommand AddAttribute { get; }
    }
}
