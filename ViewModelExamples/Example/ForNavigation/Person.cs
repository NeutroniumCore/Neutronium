using System.Windows.Input;

using MVVM.ViewModel.Infra;
using Neutronium.Core.Navigation;

namespace MVVM.ViewModel.Example.ForNavigation
{
    public class Person : MVVM.ViewModel.Example.Person, INavigable
    {
        public Person()
        {
            GoCouple = new RelayCommand(() => { Navigation?.NavigateAsync(Couple); });
        }
        public INavigationSolver Navigation { get; set; }

        private MVVM.ViewModel.Example.Couple _Couple;
        public MVVM.ViewModel.Example.Couple Couple
        {
            get { return _Couple; }
            set { Set(ref _Couple, value, "Couple"); }
        }

        public ICommand GoCouple { get; }
    }
}
