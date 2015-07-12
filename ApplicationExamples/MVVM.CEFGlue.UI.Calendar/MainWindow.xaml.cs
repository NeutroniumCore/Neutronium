using Awesomium.Core;
using MVVM.CEFGlue.ViewModel.Example;
using MVVM.CEFGlue.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MVVM.CEFGlue.UI.Calendar
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
            DataContext = new DateInformation() { Date = new DateTime(1974, 2, 26) };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.WebControl.Dispose();
        } 
    }
}
