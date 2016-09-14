using System.Windows;
using Neutronium.Example.ViewModel;

namespace Example.CefGlue.Ko.PromiseDemo
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
