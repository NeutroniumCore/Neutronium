using System;
using System.Collections.Generic;
using Example.Cfx.Spa.Routing.SetUp;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.MVVMComponents;
using Neutronium.WPF.Internal;

namespace Example.Cfx.Spa.Routing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ApplicationViewModelBuilder _ApplicationViewModelBuilder;

        private SetUpViewModel SetUp => App.SetUp;
        public bool? Debug => SetUp.Debug;
        public Uri Uri => SetUp.Uri;
        public IDictionary<string, ICommand<HTMLControlBase>> DebugCommands => SetUp.DebugCommands;

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
