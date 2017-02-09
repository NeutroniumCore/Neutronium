using Chromium.WebBrowser;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.ChromiumFx
{
    public interface IWPFCfxWebWindow : IWPFWebWindow
    {
        ChromiumWebBrowser ChromiumWebBrowser { get; }
    }
}
