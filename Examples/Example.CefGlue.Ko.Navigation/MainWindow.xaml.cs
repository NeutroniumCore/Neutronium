using System;
using System.Windows;
using System.Windows.Input;
using Neutronium.Core.Infra;
using Neutronium.Core.Navigation;
using Neutronium.Example.ViewModel.Infra;

namespace Example.CefGlue.Ko.Navigation
{
    public class Nav:  INavigable
    {
        public Nav()
        {
            DoNav = new RelayCommand(() => { Navigation?.NavigateAsync(this); });
        }

        public ICommand DoNav { get; }

        public INavigationSolver Navigation { get; set; }

    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void SetUpRoute(INavigationBuilder iNavigationBuilder)
        { 
            iNavigationBuilder.Register<Nav>("HTML\\index.html");
        }

        public MainWindow()
        {
            InitializeComponent();

            SetUpRoute(HTMLWindow.NavigationBuilder);
            var datacontext = new Nav();
            HTMLWindow.NavigateAsync(datacontext).DoNotWait();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HTMLWindow.IsDebug = !HTMLWindow.IsDebug;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.HTMLWindow.Dispose();
        } 
    }
}
