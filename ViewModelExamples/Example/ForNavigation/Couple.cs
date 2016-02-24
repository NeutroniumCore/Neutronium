using System.Windows.Input;

using MVVM.ViewModel.Infra;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Navigation;

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
            if (Navigation != null)
                Navigation.NavigateAsync(person);
        }

        public INavigationSolver Navigation { get; set; }

        public ICommand GoOne { get; private set; }

        public ICommand GoTwo { get; private set; }
    }
}
