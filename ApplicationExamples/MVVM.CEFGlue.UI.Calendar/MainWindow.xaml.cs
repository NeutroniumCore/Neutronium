using MVVM.ViewModel.Example;
using System;
using System.Windows;

namespace MVVM.Cef.Glue.UI.Calendar
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
