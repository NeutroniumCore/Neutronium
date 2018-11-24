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
    public partial class HTMLSimpleWindow : IDisposable
    {
        private readonly IWPFWebWindow _WpfWebWindow;
        private readonly string _Path;
        private UIElement _WebBrowser;
        private readonly Func<IWebView, IDisposable> _OnWebViewCreated;
        private IDisposable _Disposable;

        public HTMLSimpleWindow()
        {
            InitializeComponent();
        }

        public HTMLSimpleWindow(IWPFWebWindow wpfWebWindow, string path, Func<IWebView, IDisposable> onWebViewCreated=null)
        {
            _WpfWebWindow = wpfWebWindow;
            _Path = path;
            _OnWebViewCreated = onWebViewCreated;
            InitializeComponent();
            this.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _WebBrowser = _WpfWebWindow.UIElement;
            MainGrid.Children.Add(_WebBrowser);
            _WpfWebWindow.HTMLWindow.LoadEnd += HTMLWindow_LoadEnd;
            if ((_OnWebViewCreated != null) && (_WpfWebWindow.HTMLWindow is IModernWebBrowserWindow modern))     
                modern.BeforeJavascriptExecuted += Modern_BeforeJavascriptExecuted;

           var uri = new Uri($"{GetType().Assembly.GetPath()}\\{_Path}");
            _WpfWebWindow.HTMLWindow.NavigateTo(uri);
        }

        private void Modern_BeforeJavascriptExecuted(object sender, BeforeJavascriptExcecutionArgs e) 
        {
            var modern = _WpfWebWindow.HTMLWindow as IModernWebBrowserWindow;
            modern.BeforeJavascriptExecuted -= Modern_BeforeJavascriptExecuted;
            _Disposable = _OnWebViewCreated(e.WebView);
        }

        private void HTMLWindow_LoadEnd(object sender, Core.WebBrowserEngine.Window.LoadEndEventArgs e)
        {
            _WpfWebWindow.HTMLWindow.LoadEnd -= HTMLWindow_LoadEnd;
            Dispatcher.BeginInvoke(new Action(UpdateVisibility));
        }

        private void UpdateVisibility()
        {
            _WebBrowser.Visibility = Visibility.Visible;
            Visibility = Visibility.Visible;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Dispose();
        }

        public void Dispose()
        {
            _Disposable?.Dispose();
            _WpfWebWindow.Dispose();
            _Disposable = null;
        }
    }
}
