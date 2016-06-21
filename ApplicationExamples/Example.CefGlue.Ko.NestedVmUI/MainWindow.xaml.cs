using System.Windows;
using MVVM.ViewModel.Example;

namespace Example.CefGlue.Ko.NestedVmUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = new Couple 
            {
                One = new Person() 
                {
                    Name = null,
                    LastName = "Desmaisons",
                    Local = new Local() 
                    {
                        City = "Florianopolis",
                        Region = "SC"
                    }
                },
                Two = null
            };

            DataContext = datacontext;
        }
    }
}
