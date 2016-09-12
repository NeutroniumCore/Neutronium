using System.Windows.Input;

using MVVM.ViewModel.Infra;
using Neutronium.Core.Navigation;

namespace MVVM.ViewModel.Example.ForNavigation
{
    public class Couple : MVVM.ViewModel.Example.Couple, INavigable
    {
        public Couple() : base()
        {
            GoOne = new RelayCommand(() => Goto(One));
            GoTwo = new RelayCommand(() => Goto(Two));
        }

        private void Goto(MVVM.ViewModel.Example.Person person) 
        {
            Navigation?.NavigateAsync(person);
        }

        public INavigationSolver Navigation { get; set; }

        public ICommand GoOne { get; }

        public ICommand GoTwo { get; }
    }
}
