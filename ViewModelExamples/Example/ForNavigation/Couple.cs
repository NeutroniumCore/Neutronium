using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MVVM.CEFGlue.ViewModel.Infra;

namespace MVVM.CEFGlue.ViewModel.Example.ForNavigation
{
    public class Couple : MVVM.CEFGlue.ViewModel.Example.Couple, INavigable
    {
         public Couple():base()
        {
            GoOne = new RelayCommand(() => 
                Navigation.NavigateAsync(One));
            GoTwo = new RelayCommand(() => 
                Navigation.NavigateAsync(Two));
        }

        public INavigationSolver Navigation { get; set; }

        public ICommand GoOne { get; private set; }

        public ICommand GoTwo { get; private set; }
    }
}
