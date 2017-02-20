using Chromium.WebBrowser;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public partial class ChromiumFxControl
    {
        public ChromiumFxControl() 
        {
            InitializeComponent();
        }
        internal ChromiumWebBrowser WebBrowser => ChromiumWebBrowser;
    }
}
