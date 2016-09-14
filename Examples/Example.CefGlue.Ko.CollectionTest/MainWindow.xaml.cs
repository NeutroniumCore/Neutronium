using System.Windows;
using Example.CefGlue.Ko.CollectionTest.ViewModel;

namespace Example.CefGlue.Ko.CollectionTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ChangingCollectionViewModel();
        }
    }
}
