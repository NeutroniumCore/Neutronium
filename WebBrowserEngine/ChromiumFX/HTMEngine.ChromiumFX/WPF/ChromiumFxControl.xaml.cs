using Chromium.WebBrowser;
using System.Threading.Tasks;
using System.Windows;
using System;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public partial class ChromiumFxControl
    {
        internal ChromiumWebBrowser WebBrowser => ChromiumWebBrowser;
        private Window Window { get; set; }

        public ChromiumFxControl() 
        {
            InitializeComponent();
            this.Loaded += ChromiumFxControl_Loaded;
            ChromiumWebBrowser.BrowserCreated += ChromiumWebBrowser_BrowserCreated;
        }

        private IntPtr browserWindowHandle;
        private async void ChromiumWebBrowser_BrowserCreated(object sender, Chromium.WebBrowser.Event.BrowserCreatedEventArgs e)
        {
            browserWindowHandle = e.Browser.Host.WindowHandle;
            await Task.Delay(1000);
            var chromeWidgetMessageInterceptor = new ChromeWidgetMessageInterceptor(browserWindowHandle);
        }

        private void ChromiumFxControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ChromiumFxControl_Loaded;
            Window = Window.GetWindow(this);
            Window.StateChanged += Window_StateChanged;
            Window.Closed += Window_Closed;
        }

        private async void Window_StateChanged(object sender, EventArgs e)
        {
            if (Window.WindowState == WindowState.Minimized)
                return;

            await Task.Delay(10);
            ChromiumWebBrowser.Refresh();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Window.Closed -= Window_Closed;
            Window.StateChanged -= Window_StateChanged;
        }
    }
}
