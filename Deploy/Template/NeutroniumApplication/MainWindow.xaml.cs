using NeutroniumApplication.ViewModel;
using System.Windows;

namespace NeutroniumApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new HelloViewModel();
        }
    }
}
