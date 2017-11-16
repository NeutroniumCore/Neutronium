using System;
using System.Collections.Generic;
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
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.WebBrowserEngine.ChromiumFx.Helper;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    internal class ChromiumFxWpfWindow : IWPFCfxWebWindow
    {
        private readonly ChromiumFxControl _ChromiumFxControl;
        private readonly ChromiumWebBrowser _ChromiumWebBrowser;
        private readonly ChromiumFxControlWebBrowserWindow _ChromiumFxControlWebBrowserWindow;
        private IntPtr _DebugWindowHandle;
        private CfxClient _DebugCfxClient;
        private CfxLifeSpanHandler _DebugCfxLifeSpanHandler;

        public UIElement UIElement => _ChromiumFxControl;
        public bool IsUIElementAlwaysTopMost => true;
        public IWebBrowserWindow HTMLWindow => _ChromiumFxControlWebBrowserWindow;
        public ChromiumWebBrowser ChromiumWebBrowser => _ChromiumWebBrowser;
        public event EventHandler<DebugEventArgs> DebugToolOpened;
        private readonly List<PackUriResourceHandler> _PackPackUriResourceHandlers = new List<PackUriResourceHandler>();

        public ChromiumFxWpfWindow(IWebSessionLogger logger) 
        {
            var logger1 = logger;
            _ChromiumFxControl = new ChromiumFxControl()
            {
                Visibility = Visibility.Hidden,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ContextMenu = new ContextMenu() { Visibility = Visibility.Collapsed }
            };
            _ChromiumWebBrowser = _ChromiumFxControl.ChromiumWebBrowser;

            //add request interception to handler pack uri request
            _ChromiumWebBrowser.RequestHandler.GetResourceHandler += (s, e) =>
            {
                var uri=new Uri(e.Request.Url);
                if (uri.Scheme != "pack")
                {
                    e.SetReturnValue(null);
                    return;
                }
               
                var newResourceHandler= new PackUriResourceHandler(uri, logger1);
                _PackPackUriResourceHandlers.Add(newResourceHandler);
                e.SetReturnValue(newResourceHandler);
            };

            var dispatcher = new WPFUIDispatcher(_ChromiumFxControl.Dispatcher);
            _ChromiumFxControlWebBrowserWindow = new ChromiumFxControlWebBrowserWindow(_ChromiumWebBrowser, dispatcher, logger1);         
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
            DebugToolOpened?.Invoke(this, new DebugEventArgs(true));

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
            DebugToolOpened?.Invoke(this, new DebugEventArgs(false));
        }

        private void DebugLifeSpan_OnAfterCreated(object sender, CfxOnAfterCreatedEventArgs e)
        {
            _DebugWindowHandle = e.Browser.Host.WindowHandle;
        }

        public void CloseDebugTools() 
        {
            _ChromiumWebBrowser.BrowserHost.CloseDevTools();
            DebugToolOpened?.Invoke(this, new DebugEventArgs(false));
        }

        public void Dispose()
        {
            _ChromiumWebBrowser.Dispose();
            _PackPackUriResourceHandlers.Clear();
            _DebugCfxClient = null;
            _DebugCfxLifeSpanHandler = null;
        }
    }
}
