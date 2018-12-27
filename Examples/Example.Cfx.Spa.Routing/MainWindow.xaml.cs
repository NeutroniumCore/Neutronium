using System;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Example.Cfx.Spa.Routing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ApplicationViewModelBuilder _ApplicationViewModelBuilder;

        public bool? Debug => App.MainApplication.Debug;
        public Uri Uri => App.MainApplication.Uri;

        public MainWindow()
        {
            Initialized += MainWindow_Initialized;
            InitializeComponent();
            HtmlView.OnDebugMenuOpening += HtmlView_OnDebugMenuOpening;
        }

        private void HtmlView_OnDebugMenuOpening(object sender, Neutronium.WPF.DebugMenuOpeningEvent e)
        {
            e.AdditionalMenuItems.Add(new ContextMenuItem(() => HtmlView.ReloadAsync(), "Reload", true));
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
