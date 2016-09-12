using System.Windows.Input;
using Neutronium.Core.Navigation;
using Neutronium.Example.ViewModel.Infra;

namespace Neutronium.Example.ViewModel.ForNavigation
{
    public class Person : Neutronium.Example.ViewModel.Person, INavigable
    {
        public Person()
        {
            GoCouple = new RelayCommand(() => { Navigation?.NavigateAsync(Couple); });
        }
        public INavigationSolver Navigation { get; set; }

        private Neutronium.Example.ViewModel.Couple _Couple;
        public Neutronium.Example.ViewModel.Couple Couple
        {
            get { return _Couple; }
            set { Set(ref _Couple, value, "Couple"); }
        }

        public ICommand GoCouple { get; }
    }
}
