using System;
using System.Windows;
using Neutronium.Example.ViewModel;

namespace Example.CefGlue.Ko.Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
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
            WebControl.Dispose();
        } 
    }
}
