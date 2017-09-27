using System.Dynamic;
using Neutronium.Example.ViewModel.Infra;

namespace Example.Dictionary.Cfx.Vue.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        public MainViewModel(ExpandoObject child)
        {
            Dynamic = child;
        }

        public ExpandoObject Dynamic { get; set; }
    }
}
