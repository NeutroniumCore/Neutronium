using System;
using System.Windows;
using Neutronium.WPF.ViewModel;

namespace Example.ChromiumFx.Vue.Chromeless
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var dataContext = new
            {
                Window = new WindowViewModel(this)
            };
            DataContext = dataContext;
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WcBrowser.Dispose();
        }
    }
}
