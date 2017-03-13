using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.CefGlue.CefSession;
using Neutronium.WebBrowserEngine.CefGlue.WindowImplementation;
using Neutronium.WPF;
using System;
using Neutronium.Core.WebBrowserEngine.Control;

namespace Neutronium.WebBrowserEngine.CefGlue
{
    internal class CefGlueWPFWebWindow : IWPFWebWindow
    {
        public IWebBrowserWindow HTMLWindow => _WpfCefBrowser;
        public UIElement UIElement => _WpfCefBrowser;
        public bool IsUIElementAlwaysTopMost => false;
        event EventHandler<DebugEventArgs> IWPFWebWindow.DebugToolOpened { add { } remove { } }

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

        public void Inject(Key KeyToInject)
        {
            _WpfCefBrowser.Inject(KeyToInject);
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
