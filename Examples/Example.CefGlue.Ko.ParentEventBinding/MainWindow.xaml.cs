using System.Windows;
using Neutronium.Example.ViewModel;

namespace Example.CefGlue.Ko.ParentEventBinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var dt = new CardViewModel();
            dt.Items.Add(new ItemViewModel() { Name = "P1", Price = 5.2M });
            InitializeComponent();
            DataContext = dt;
        }
    }
}
