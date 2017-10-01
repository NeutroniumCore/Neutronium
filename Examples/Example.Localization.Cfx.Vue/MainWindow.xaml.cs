using System;
using Example.Localization.Cfx.Vue.ViewModel;

namespace Example.Localization.Cfx.Vue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.wcBrowser.Dispose();
            base.OnClosed(e);
        }
    }
}
