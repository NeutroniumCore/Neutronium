using Chromium.WebBrowser;

namespace HTMEngine.ChromiumFX.WPF
{
    public partial class ChromiumFxControl
    {
        public ChromiumFxControl() 
        {
            InitializeComponent();
        }

        public ChromiumWebBrowser WebBrowser
        {
            get { return ChromiumWebBrowser; }
        }
    }
}
