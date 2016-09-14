using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Awesomium.Core;
using Awesomium.Windows.Controls;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.Awesomium;
using Neutronium.WPF;

namespace HTMLEngine.Awesomium
{
    internal class AwesomiumWPFWebWindow : IWPFWebWindow
    {
        private readonly WebSession _Session;
        private readonly WebConfig _WebConfig;
        private readonly WebControl _WebControl;
        private readonly AwesomiumHTMLWindow _AwesomiumHTMLWindow;

        public IWebBrowserWindow HTMLWindow => _AwesomiumHTMLWindow;
        public UIElement UIElement => _WebControl;
        public bool IsUIElementAlwaysTopMost => false;

        public AwesomiumWPFWebWindow(WebSession iSession, WebConfig webConfig)
        {
            _Session = iSession;
            _WebConfig = webConfig;

            _WebControl = new WebControl()
            {
                WebSession = _Session,
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                ContextMenu = new ContextMenu() { Visibility = Visibility.Collapsed }
            };

            _AwesomiumHTMLWindow = new AwesomiumHTMLWindow(_WebControl);     
        }

        public void Inject(Key KeyToInject)
        {
            IWebView wv = _WebControl;
            var kev = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, KeyToInject);
            wv.InjectKeyboardEvent(kev.GetKeyboardEvent(WebKeyboardEventType.KeyDown));
        }

        public bool OnDebugToolsRequest() 
        {
            var port = _WebConfig.RemoteDebuggingPort;
            if (port == 0)
                return false;

            ProcessHelper.OpenLocalUrlInBrowser(port);
            return true;
        }

        public void CloseDebugTools() 
        {
        }

        public void Dispose()
        {
            _WebControl.Dispose();
            _AwesomiumHTMLWindow.Dispose();
        }
    }
}
