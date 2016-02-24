using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MVVM.HTML.Core;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Navigation;
using MVVM.ViewModel.Infra;

namespace MVVM.Cef.Glue.AnimatedNavigation
{
    public class Nav:  INavigable
    {
        public Nav()
        {
            DoNav = new RelayCommand(() => { if (Navigation != null) Navigation.NavigateAsync(this); });
        }

        public ICommand DoNav { get; private set;}

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
