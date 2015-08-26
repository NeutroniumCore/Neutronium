using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

using MVVM.ViewModel.Infra;
using MVVM.HTML.Core;

namespace MVVM.ViewModel.Example.ForNavigation
{
    public class Person : MVVM.ViewModel.Example.Person, INavigable
    {
        public Person():base()
        {
            GoCouple = new RelayCommand(() => Navigation.NavigateAsync(Couple));
        }
        public INavigationSolver Navigation { get; set; }

        private MVVM.ViewModel.Example.Couple _Couple;
        public MVVM.ViewModel.Example.Couple Couple
        {
            get { return _Couple; }
            set { Set(ref _Couple, value, "Couple"); }
        }

        public ICommand GoCouple { get; private set; }
    }
}
