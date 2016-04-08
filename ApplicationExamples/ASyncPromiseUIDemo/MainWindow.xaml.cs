using MVVM.ViewModel.Example;
using System.Windows;

namespace ASyncPromiseUIDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        { 
            InitializeComponent();
            DataContext = new Factory();
        }
    }
}
