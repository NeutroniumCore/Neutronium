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
using MVVM.CEFGlue;


namespace MVVM.Navigation2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
    

        private void SetUpRoute(INavigationBuilder iNavigationBuilder)
        {
            iNavigationBuilder.Register<object>("HTMLUI\\index_one.html");
            iNavigationBuilder.Register<MVVM.CEFGlue.ViewModel.Example.ForNavigation.Person>("HTMLUI\\index_one.html");
            iNavigationBuilder.Register<MVVM.CEFGlue.ViewModel.Example.ForNavigation.Couple>("HTMLUI\\index_couple.html");
        }

        public MainWindow()
        {
            InitializeComponent();

            HTMLWindow.UseINavigable = true;
            SetUpRoute(HTMLWindow.NavigationBuilder);
            
            var datacontext = new MVVM.CEFGlue.ViewModel.Example.ForNavigation.Couple();
            var my = new MVVM.CEFGlue.ViewModel.Example.ForNavigation.Person()
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new MVVM.CEFGlue.ViewModel.Example.Local() { City = "Florianopolis", Region = "SC" }
            };
            my.Couple = datacontext;
            datacontext.One = my;

            HTMLWindow.NavigateAsync(datacontext);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.HTMLWindow.Dispose();
        } 
    }
}
