using MVVM.CEFGlue.UI.CollectionTest.ViewModel;
using System.Windows;

namespace MVVM.Cef.Glue.UI.CollectionTest
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
