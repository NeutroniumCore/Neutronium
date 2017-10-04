using System.Windows.Input;
using Neutronium.Core.Navigation;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.Example.ViewModel.ForNavigation
{
    public class Person : Neutronium.Example.ViewModel.Person, INavigable
    {
        public Person()
        {
            GoCouple = new RelaySimpleCommand(() => { Navigation?.NavigateAsync(Couple); });
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
