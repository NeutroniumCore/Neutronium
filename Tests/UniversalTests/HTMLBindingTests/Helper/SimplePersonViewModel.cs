using Neutronium.Example.ViewModel;
using Neutronium.Example.ViewModel.Infra;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class SimplePersonViewModel : ViewModelBase
    {
        private PersonalState _PersonalState;
        public PersonalState PersonalState
        {
            get => _PersonalState;
            set => Set(ref _PersonalState, value, "PersonalState");
        }
    }
}
