using System.Windows.Input;
using Neutronium.Core.Navigation;
using Neutronium.Example.ViewModel.Infra;

namespace Neutronium.Example.ViewModel.ForNavigation
{
    public class Couple : Neutronium.Example.ViewModel.Couple, INavigable
    {
        public Couple() : base()
        {
            GoOne = new RelayCommand(() => Goto(One));
            GoTwo = new RelayCommand(() => Goto(Two));
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
