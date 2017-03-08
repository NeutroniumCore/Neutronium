using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Chromium;
using Chromium.WebBrowser;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.ChromiumFx.WPF;
using Neutronium.WPF.Internal;
using Chromium.Event;
using Neutronium.WebBrowserEngine.ChromiumFx.Helper;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    internal class ChromiumFxWpfWindow : IWPFCfxWebWindow
    {
        private readonly ChromiumFxControl _ChromiumFxControl;
        private readonly ChromiumWebBrowser _ChromiumWebBrowser;
        private readonly ChromiumFxControlWebBrowserWindow _chromiumFxControlWebBrowserWindow;
        private readonly IWebSessionLogger _Logger;
        private IntPtr _DebugWindowHandle;
        private CfxClient _DebugCfxClient;
        private CfxLifeSpanHandler _DebugCfxLifeSpanHandler;

        public UIElement UIElement => _ChromiumFxControl;
        public bool IsUIElementAlwaysTopMost => true;
        public IWebBrowserWindow HTMLWindow => _chromiumFxControlWebBrowserWindow;
        public ChromiumWebBrowser ChromiumWebBrowser => _ChromiumWebBrowser;

        public ChromiumFxWpfWindow(IWebSessionLogger logger) 
        {
            _Logger = logger;
            _ChromiumFxControl = new ChromiumFxControl()
            {
                Visibility = Visibility.Hidden,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                //ContextMenu = new ContextMenu() { Visibility = Visibility.Collapsed }
            };
            _ChromiumWebBrowser = _ChromiumFxControl.ChromiumWebBrowser;
            var dispatcher = new WPFUIDispatcher(_ChromiumFxControl.Dispatcher);
            _chromiumFxControlWebBrowserWindow = new ChromiumFxControlWebBrowserWindow(_ChromiumWebBrowser, dispatcher, _Logger);
        }

        public void Inject(Key keyToInject) 
        {
            var cxKeyEvent = new CfxKeyEvent() 
            {
                WindowsKeyCode = (int) keyToInject
            };

            _ChromiumWebBrowser.Browser.Host.SendKeyEvent(cxKeyEvent);
        }

        public bool OnDebugToolsRequest() 
        {
            if (_DebugWindowHandle != IntPtr.Zero)
            {
                NativeWindowHelper.BringToFront(_DebugWindowHandle);
                return true;
            }
            var windowInfo = new CfxWindowInfo {
                Style = Chromium.WindowStyle.WS_OVERLAPPEDWINDOW | Chromium.WindowStyle.WS_CLIPCHILDREN | Chromium.WindowStyle.WS_CLIPSIBLINGS | Chromium.WindowStyle.WS_VISIBLE,
                ParentWindow = IntPtr.Zero,
                WindowName = "Neutronium Chromium Dev Tools",
                X = 200,
                Y = 200,
                Width = 800,
                Height = 600
            };

            _DebugCfxClient = new CfxClient();
            _DebugCfxClient.GetLifeSpanHandler += DebugClient_GetLifeSpanHandler;
            _ChromiumWebBrowser.BrowserHost.ShowDevTools(windowInfo, _DebugCfxClient, new CfxBrowserSettings(), null);
            return true;
        }

        private void DebugClient_GetLifeSpanHandler(object sender, CfxGetLifeSpanHandlerEventArgs e)
        {
            if (_DebugCfxLifeSpanHandler == null)
            {
                _DebugCfxLifeSpanHandler = new CfxLifeSpanHandler();
                _DebugCfxLifeSpanHandler.OnAfterCreated += DebugLifeSpan_OnAfterCreated;
                _DebugCfxLifeSpanHandler.OnBeforeClose += DebugLifeSpan_OnBeforeClose;
            }
            e.SetReturnValue(_DebugCfxLifeSpanHandler);
        }

        private void DebugLifeSpan_OnBeforeClose(object sender, CfxOnBeforeCloseEventArgs e)
        {
            _DebugWindowHandle = IntPtr.Zero;
            _DebugCfxClient = null;
        }

        private void DebugLifeSpan_OnAfterCreated(object sender, CfxOnAfterCreatedEventArgs e)
        {
            _DebugWindowHandle = e.Browser.Host.WindowHandle;
        }

        public void CloseDebugTools() 
        {
            _ChromiumWebBrowser.BrowserHost.CloseDevTools();
        }

        public void Dispose()
        {
            _ChromiumWebBrowser.Dispose();
            _DebugCfxClient = null;
            _DebugCfxLifeSpanHandler = null;
        }
    }
}
