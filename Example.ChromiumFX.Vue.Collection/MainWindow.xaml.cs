using Example.ChromiumFX.Vue.Collection.VM;
using MVVM.ViewModel.Example;
using System.Windows;

namespace Example.ChromiumFX.Vue.Collection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var lvm = new ListVM(new Person() { Name="John"}, new Person() { Name = "Jean" }, new Person() { Name = "Fritz" });
            DataContext = lvm;
        }
    }
}
