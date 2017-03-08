using Neutronium.Core.Infra;
using System;
using System.Windows;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.WPF.Internal.DebugTools
{
    /// <summary>
    /// Interaction logic for ViewModelDebug.xaml
    /// </summary>
    public partial class HTMLSimpleWindow : Window
    {
        private readonly IWPFWebWindow _WPFWebWindow;
        private readonly string _path;
        private UIElement _WebBrowser;
        private Action<IWebView> _OnWebViewCreated;

        public HTMLSimpleWindow()
        {
            InitializeComponent();
        }

        public HTMLSimpleWindow(IWPFWebWindow wpfWebWindow, string path, Action<IWebView> onWebViewCreated)
        {
            _WPFWebWindow = wpfWebWindow;
            _path = path;
            _OnWebViewCreated = onWebViewCreated;
            InitializeComponent();
            this.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _WebBrowser = _WPFWebWindow.UIElement;
            MainGrid.Children.Add(_WebBrowser);
            _WPFWebWindow.HTMLWindow.LoadEnd += HTMLWindow_LoadEnd;
            var modern = _WPFWebWindow.HTMLWindow as IModernWebBrowserWindow;
            if ((_OnWebViewCreated != null) && (modern!=null))     
                modern.BeforeJavascriptExecuted += Modern_BeforeJavascriptExecuted;

           var uri = new Uri($"{GetType().Assembly.GetPath()}\\{_path}");
            _WPFWebWindow.HTMLWindow.NavigateTo(uri);
        }

        private void Modern_BeforeJavascriptExecuted(object sender, BeforeJavascriptExcecutionArgs e) 
        {
            var modern = _WPFWebWindow.HTMLWindow as IModernWebBrowserWindow;
            modern.BeforeJavascriptExecuted -= Modern_BeforeJavascriptExecuted;
            _OnWebViewCreated(e.WebView);
        }

        private void HTMLWindow_LoadEnd(object sender, Core.WebBrowserEngine.Window.LoadEndEventArgs e)
        {
            _WPFWebWindow.HTMLWindow.LoadEnd -= HTMLWindow_LoadEnd;
            _WebBrowser.Visibility = Visibility.Visible;
            Visibility = Visibility.Visible;
        }
    }
}
