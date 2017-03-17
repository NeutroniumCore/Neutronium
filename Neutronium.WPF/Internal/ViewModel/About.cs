using System.Windows.Input;
using Neutronium.Core.Infra.VM;
using Neutronium.Core.Infra;

namespace Neutronium.WPF.Internal.DebugTools
{

    public class About
    {
        public string CoreVersion { get; set; }
        public string WPFVersion { get; set; }
        public string WebBrowser { get; set; }
        public string JavascriptFrameworkVersion { get; set; }
        public string WebBrowserVersion { get; set; }
        public string JavascriptFramework { get; set; }
        public string BrowserBinding { get; set; }
        public string MVVMBinding { get; set; }
        public ICommand GoToGithub { get; }

        public About()
        {
            GoToGithub = new BasicRelayCommand(() => ProcessHelper.OpenUrlInBrowser(@"https://github.com/NeutroniumCore/Neutronium"));
        }
    }
}
