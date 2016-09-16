using System.Windows;
using Neutronium.Example.ViewModel;

namespace Examples.ChromiumFx.Vue.PromiseDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Factory();
        }
    }
}
