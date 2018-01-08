using System.Windows;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Neutronium.WebBrowserEngine.ChromiumFx.Util;
using Chromium.Event;
using Neutronium.WPF;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF {
    public partial class ChromiumFxControl
    {
        private Window Window { get; set; }
        private IntPtr WindowHandle { get; set; }
        private IntPtr FormHandle => _BrowserHandle;
        private IntPtr _ChromeWidgetHostHandle;
        private BrowserWidgetMessageInterceptor _ChromeWidgetMessageInterceptor;
        private Region _DraggableRegion = null;
        private Rectangle _Rectange = new Rectangle(0,0,0,0);
        private IntPtr _BrowserHandle;

        public ChromiumFxControl()
        {
            InitializeComponent();
            this.Loaded += ChromiumFxControl_Loaded;
            ChromiumWebBrowser.BrowserCreated += ChromiumWebBrowser_BrowserCreated;
            ChromiumWebBrowser.RequestHandler.OnQuotaRequest += RequestHandler_OnQuotaRequest;

            var dragHandler = ChromiumWebBrowser.DragHandler;
            dragHandler.OnDragEnter += (o, e) => { e.SetReturnValue(true); };
            dragHandler.OnDraggableRegionsChanged += DragHandler_OnDraggableRegionsChanged;
        }

        private void RequestHandler_OnQuotaRequest(object sender, CfxOnQuotaRequestEventArgs e)
        {
            e.SetReturnValue(true);
        }

        private void DragHandler_OnDraggableRegionsChanged(object sender, Chromium.Event.CfxOnDraggableRegionsChangedEventArgs args)
        {
            _DraggableRegion = args.Regions.Aggregate(_DraggableRegion, (current, region) =>
            {
                var rect = new Rectangle(region.Bounds.X, region.Bounds.Y, region.Bounds.Width, region.Bounds.Height);
                if (current == null)
                    return new Region(rect);

                if (region.Draggable)
                    current.Union(rect);
                else
                    current.Exclude(rect);

                return current;
            });

            _Rectange = args.Regions.Select(r => r.Bounds).Aggregate(_Rectange, (current, bounds) =>
                new Rectangle( 0, 0, Math.Max(current.X + current.Size.Width, bounds.X + bounds.Width),
                                     Math.Max(current.Y + current.Size.Height, bounds.X + bounds.Height))
            );
        }

        private async void ChromiumWebBrowser_BrowserCreated(object sender, Chromium.WebBrowser.Event.BrowserCreatedEventArgs e)
        {
            _BrowserHandle = e.Browser.Host.WindowHandle;

            var resilientGetHandle = new Resilient(() => ChromeWidgetHandleFinder.TryFindHandle(_BrowserHandle, out _ChromeWidgetHostHandle));

            await resilientGetHandle.WithTimeOut(100).StartIn(100);
            _ChromeWidgetMessageInterceptor = new BrowserWidgetMessageInterceptor(this.ChromiumWebBrowser, _ChromeWidgetHostHandle, OnWebBroswerMessage);
        }

        private void ChromiumFxControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ChromiumFxControl_Loaded;
            Window = Window.GetWindow(this);
            WindowHandle = new System.Windows.Interop.WindowInteropHelper(Window).Handle;
            Window.Closed += Window_Closed;
        }

        //private async void Window_StateChanged(object sender, EventArgs e)
        //{
        //    if (Window.WindowState == WindowState.Minimized)
        //        return;
        //    await Task.Delay(10);
        //    ChromiumWebBrowser.Refresh();
        //}

        private bool OnWebBroswerMessage(Message message)
        {
            switch (message.Msg)
            {
                case NativeMethods.WindowsMessage.WM_LBUTTONDBLCLK:
                    if (!IsInDragRegion(GetPoint(message.LParam)))
                        break;

                    Dispatcher.Invoke(ToogleMaximize);
                    return true;

                case NativeMethods.WindowsMessage.WM_LBUTTONDOWN:
                    var point = GetPoint(message.LParam);
                    var underDragZone = IsInDragRegion(point);
                    if (!underDragZone)
                        break;

                    NativeMethods.ReleaseCapture();
                    NativeMethods.SendMessage(WindowHandle, 0xA1, (IntPtr) 0x2, (IntPtr) 0);
                    return true;
            }
            return false;
        }

        private bool IsInDragRegion(System.Windows.Point point)
        {
            return _DraggableRegion?.IsVisible(Convert(point)) == true;
        }

        private void ToogleMaximize()
        {
            Window.WindowState = (Window.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private static System.Windows.Point GetPoint(IntPtr lparam)
        {
            var x = NativeMethods.LoWord(lparam.ToInt32());
            var y = NativeMethods.HiWord(lparam.ToInt32());
            return new System.Windows.Point(x, y);
        }

        private static System.Drawing.Point Convert(System.Windows.Point point)
        {
            return new System.Drawing.Point((int)point.X, (int)point.Y);
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Window.Closed -= Window_Closed;
            _ChromeWidgetMessageInterceptor?.ReleaseHandle();
            _ChromeWidgetMessageInterceptor?.DestroyHandle();
            _ChromeWidgetMessageInterceptor = null;
        }
    }
}
