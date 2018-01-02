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
            Initialized += MainWindow_Initialized;
            InitializeComponent();
        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            var dataContext = new
            {
                Window = new WindowViewModel(this)
            };
            DataContext = dataContext;
            Initialized -= MainWindow_Initialized;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WcBrowser.Dispose();
        }
    }
}
