using System.Windows;
using MVVM.ViewModel.Example;

namespace MVVM.Cef.Glue.UI2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = new Couple();
            datacontext.One =  new Person()
            {
                Name = null,
                LastName = "Desmaisons",
                Local = new Local() { City = "Florianopolis", Region = "SC" }
            };
            datacontext.Two = null;

            DataContext = datacontext;
        }
    }
}
