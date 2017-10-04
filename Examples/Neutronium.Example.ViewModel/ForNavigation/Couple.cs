using System.Windows.Input;
using Neutronium.Core.Navigation;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.Example.ViewModel.ForNavigation
{
    public class Couple : ViewModel.Couple, INavigable
    {
        public Couple() : base()
        {
            GoOne = new RelaySimpleCommand(() => Goto(One));
            GoTwo = new RelaySimpleCommand(() => Goto(Two));
        }

        private void Goto(Neutronium.Example.ViewModel.Person person) 
        {
            Navigation?.NavigateAsync(person);
        }

        public INavigationSolver Navigation { get; set; }

        public ICommand GoOne { get; }

        public ICommand GoTwo { get; }
    }
}
