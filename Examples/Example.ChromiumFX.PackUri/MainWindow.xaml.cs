using System;
using System.Windows;

namespace Example.ChromiumFX.PackUri
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.InitializeComponent();

            DataContext = new {text="Hello from resource"};
        }

        protected override void OnClosed(EventArgs e)
        {
            this.wcBrowser.Dispose();
            base.OnClosed(e);
        }
    }
}
