using System;
using Example.Cfx.Spa.Routing.SetUp;

namespace Example.Cfx.Spa.Routing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ApplicationViewModelBuilder _ApplicationViewModelBuilder;

        public SetUpViewModel SetUp => App.SetUp;
 
        public MainWindow()
        {
            Initialized += MainWindow_Initialized;          
            InitializeComponent();
        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            DataContext = BuildApplicationViewModel();
            Initialized -= MainWindow_Initialized;
        }

        private ApplicationViewModel BuildApplicationViewModel()
        {
            _ApplicationViewModelBuilder = new ApplicationViewModelBuilder(this);
            return _ApplicationViewModelBuilder.ApplicationViewModel;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            HtmlView.Dispose();
        }
    }
}
