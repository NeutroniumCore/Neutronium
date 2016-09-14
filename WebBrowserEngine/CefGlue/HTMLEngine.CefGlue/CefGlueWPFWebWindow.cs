using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.CefGlue.CefSession;
using Neutronium.WebBrowserEngine.CefGlue.WindowImplementation;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.CefGlue
{
    internal class CefGlueWPFWebWindow : IWPFWebWindow
    {
        private readonly WpfCefBrowser _WpfCefBrowser;
        internal CefGlueWPFWebWindow(NeutroniumCefApp app)
        {
            _WpfCefBrowser = new WpfCefBrowser(app)
            {
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                ContextMenu = new ContextMenu() { Visibility = Visibility.Collapsed }
            };
        }

        public IWebBrowserWindow HTMLWindow
        {
            get { return _WpfCefBrowser; }
        }

        public void Inject(Key KeyToInject)
        {
            _WpfCefBrowser.Inject(KeyToInject);
        }

        public UIElement UIElement
        {
            get { return _WpfCefBrowser; }
        }

        public bool IsUIElementAlwaysTopMost 
        {
            get { return false; }
        }

        public bool OnDebugToolsRequest() 
        {
            var port = CefCoreSessionSingleton.Get().CefSettings.RemoteDebuggingPort;
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
            _WpfCefBrowser.Dispose();
        }
    }
}
