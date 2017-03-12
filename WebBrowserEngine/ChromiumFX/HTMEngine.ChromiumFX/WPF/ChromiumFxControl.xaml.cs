using Chromium.WebBrowser;
using System.Windows;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public partial class ChromiumFxControl
    {
        internal ChromiumWebBrowser WebBrowser => ChromiumWebBrowser;
        private Window Window { get; set; }
        private bool _FirstActived = true;

        public ChromiumFxControl() 
        {
            InitializeComponent();
            this.Loaded += ChromiumFxControl_Loaded;
        }

        private void ChromiumFxControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ChromiumFxControl_Loaded;
            Window = Window.GetWindow(this);
            Window.Closed += Window_Closed;
            Window.Activated += Window_Activated;
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            if (_FirstActived)
            {
                _FirstActived = false;
                return;
            }
            //Host.InvalidateArrange();
            System.Action refresh = () => { ChromiumWebBrowser.Invalidate(); };
            this.Dispatcher.BeginInvoke(refresh);
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Window.Closed -= Window_Closed;
            Window.Activated -= Window_Closed;
        }
    }
}
