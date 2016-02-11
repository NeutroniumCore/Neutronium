using System;
using System.Windows;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Infra;

namespace MVVM.Navigation2
{
    public partial class MainWindow : Window
    {
        private void SetUpRoute(INavigationBuilder iNavigationBuilder)
        {
            iNavigationBuilder.Register<object>("HTMLUI\\index_one.html");
            iNavigationBuilder.Register<MVVM.ViewModel.Example.ForNavigation.Person>("HTMLUI\\index_one.html");
            iNavigationBuilder.Register<MVVM.ViewModel.Example.ForNavigation.Couple>("HTMLUI\\index_couple.html");
        }

        public MainWindow()
        {
            InitializeComponent();

            HTMLWindow.UseINavigable = true;
            SetUpRoute(HTMLWindow.NavigationBuilder);
            
            var datacontext = new MVVM.ViewModel.Example.ForNavigation.Couple();
            var my = new MVVM.ViewModel.Example.ForNavigation.Person()
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new MVVM.ViewModel.Example.Local() { City = "Florianopolis", Region = "SC" }
            };
            my.Couple = datacontext;
            datacontext.One = my;

            HTMLWindow.NavigateAsync(datacontext).DoNotWait();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.HTMLWindow.Dispose();
        }

        private void HTMLWindow_Loaded(object sender, RoutedEventArgs e)
        {
        } 
    }
}
