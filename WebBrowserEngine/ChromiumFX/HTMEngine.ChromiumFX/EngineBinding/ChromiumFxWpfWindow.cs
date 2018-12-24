using Chromium;
using Chromium.Event;
using Chromium.WebBrowser;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.ChromiumFx.Helper;
using Neutronium.WebBrowserEngine.ChromiumFx.WPF;
using Neutronium.WPF.Internal;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowStyle = Chromium.WindowStyle;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    internal class ChromiumFxWpfWindow : IWPFCfxWebWindow
    {
        private readonly ChromiumFxControl _ChromiumFxControl;
        private readonly ChromiumWebBrowser _ChromiumWebBrowser;
        private readonly IWebSessionLogger _Logger;
        private readonly ChromiumFxControlWebBrowserWindow _ChromiumFxControlWebBrowserWindow;
        private IntPtr _DebugWindowHandle = IntPtr.Zero;
        private CfxClient _DebugCfxClient;
        private CfxLifeSpanHandler _DebugCfxLifeSpanHandler;
        private CfxDisplayHandler _DisplayHandler;
        private CfxBrowserHost _BrowserHost;

        public UIElement UIElement => _ChromiumFxControl;
        public bool IsUIElementAlwaysTopMost => true;
        public IWebBrowserWindow HTMLWindow => _ChromiumFxControlWebBrowserWindow;
        public ChromiumWebBrowser ChromiumWebBrowser => _ChromiumWebBrowser;
        public event EventHandler<DebugEventArgs> DebugToolOpened;

        public ChromiumFxWpfWindow(IWebSessionLogger logger)
        {
            _Logger = logger;
            _ChromiumFxControl = new ChromiumFxControl()
            {
                Visibility = Visibility.Hidden,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ContextMenu = new ContextMenu() { Visibility = Visibility.Collapsed }
            };
            _ChromiumWebBrowser = _ChromiumFxControl.ChromiumWebBrowser;
            _ChromiumWebBrowser.LoadHandler.OnLoadEnd += LoadHandler_OnLoadEnd;
            var dispatcher = new WPFUIDispatcher(_ChromiumFxControl.Dispatcher);
            _ChromiumFxControlWebBrowserWindow = new ChromiumFxControlWebBrowserWindow(_ChromiumWebBrowser, dispatcher, logger);
        }

        private void LoadHandler_OnLoadEnd(object sender, CfxOnLoadEndEventArgs e)
        {
            //Important browserHost may change in some corner cases
            _BrowserHost = _BrowserHost ?? _ChromiumWebBrowser.BrowserHost;
        }

        public void Inject(Key keyToInject)
        {
            var cxKeyEvent = new CfxKeyEvent()
            {
                WindowsKeyCode = (int)keyToInject
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

            DisplayDebug();
            DebugToolOpened?.Invoke(this, new DebugEventArgs(true));
            return true;
        }

        private void DisplayDebug()
        {
            const WindowStyle style = WindowStyle.WS_OVERLAPPEDWINDOW | WindowStyle.WS_CLIPCHILDREN |
                                      WindowStyle.WS_CLIPSIBLINGS | WindowStyle.WS_VISIBLE;
            var cfxWindowInfo = new CfxWindowInfo
            {
                Style = style,
                ParentWindow = IntPtr.Zero,
                WindowName = "Neutronium Chromium Dev Tools",
                X = 200,
                Y = 200,
                Width = 800,
                Height = 600
            };

            _DebugCfxClient = new CfxClient();
            _DebugCfxClient.GetDisplayHandler += _DebugCfxClient_GetDisplayHandler;
            _DebugCfxClient.GetLifeSpanHandler += DebugClient_GetLifeSpanHandler;
            _BrowserHost.ShowDevTools(cfxWindowInfo, _DebugCfxClient, new CfxBrowserSettings(), null);
        }

        private void _DebugCfxClient_GetDisplayHandler(object sender, CfxGetDisplayHandlerEventArgs e)
        {
            if (_DisplayHandler == null)
            {
                _DisplayHandler = new CfxDisplayHandler();
                _DisplayHandler.OnConsoleMessage += Handler_OnConsoleMessage;
            }
            e.SetReturnValue(_DisplayHandler);
        }

        private void Handler_OnConsoleMessage(object sender, CfxOnConsoleMessageEventArgs e)
        {
            e.SetReturnValue(true);
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
            if (_DebugWindowHandle == IntPtr.Zero)
                return;

            _DebugWindowHandle = IntPtr.Zero;
            _DebugCfxClient = null;
            DebugToolOpened?.Invoke(this, new DebugEventArgs(false));
        }

        private void DebugLifeSpan_OnAfterCreated(object sender, CfxOnAfterCreatedEventArgs e)
        {
            _DebugWindowHandle = e.Browser.Host.WindowHandle;
        }

        public void CloseDebugTools()
        {
            _BrowserHost.CloseDevTools();
            DebugToolOpened?.Invoke(this, new DebugEventArgs(false));
        }

        public void Dispose()
        {
            _ChromiumWebBrowser.Dispose();
            _DebugCfxClient = null;
            _DebugCfxLifeSpanHandler = null;
        }
    }
}
