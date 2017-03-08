using Neutronium.Core.Infra;
using System;
using System.Windows;

namespace Neutronium.WPF.Internal.DebugTools
{
    /// <summary>
    /// Interaction logic for ViewModelDebug.xaml
    /// </summary>
    public partial class ViewModelDebug : Window
    {
        private readonly IWPFWebWindow _WPFWebWindow;
        private readonly string _path;
        private UIElement _WebBrowser;

        public ViewModelDebug()
        {
            InitializeComponent();
        }

        public ViewModelDebug(IWPFWebWindow wpfWebWindow, string path)
        {
            _WPFWebWindow = wpfWebWindow;
            _path = path;
            InitializeComponent();
            this.Loaded += ViewModelDebug_Loaded;
        }

        private void ViewModelDebug_Loaded(object sender, RoutedEventArgs e)
        {
            _WebBrowser = _WPFWebWindow.UIElement;
            MainGrid.Children.Add(_WebBrowser);
            _WPFWebWindow.HTMLWindow.LoadEnd += HTMLWindow_LoadEnd;
            var uri = new Uri($"{GetType().Assembly.GetPath()}\\{_path}");
            _WPFWebWindow.HTMLWindow.NavigateTo(uri);
        }

        private void HTMLWindow_LoadEnd(object sender, Core.WebBrowserEngine.Window.LoadEndEventArgs e)
        {
            _WebBrowser.Visibility = Visibility.Visible;
            Visibility = Visibility.Visible;
        }
    }
}
