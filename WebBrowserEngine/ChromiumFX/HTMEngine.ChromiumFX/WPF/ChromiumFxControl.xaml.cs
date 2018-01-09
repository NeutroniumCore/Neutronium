using System.Windows;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Neutronium.WebBrowserEngine.ChromiumFx.Util;
using Chromium.Event;
using Neutronium.WPF;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public partial class ChromiumFxControl
    {
        private Window Window { get; set; }
        private IntPtr WindowHandle { get; set; }
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
            Window.StateChanged += Window_StateChanged;
            Window.Closed += Window_Closed;
        }

        private async void Window_StateChanged(object sender, EventArgs e)
        {
            if (Window.WindowState == WindowState.Minimized)
                return;

            await Task.Delay(10);
            ChromiumWebBrowser.Refresh();
        }

        private bool OnWebBroswerMessage(Message message)
        {
            switch (message.Msg)
            {
                case NativeMethods.WindowsMessage.WM_LBUTTONDBLCLK:
                    if (!IsInDragRegion(GetPoint(message)))
                        break;

                    Dispatcher.Invoke(ToogleMaximize);
                    return true;

                case NativeMethods.WindowsMessage.WM_LBUTTONDOWN:
                    var point = GetPoint(message);
                    var underDragZone = IsInDragRegion(point);
                    if (!underDragZone)
                        return false;

                    NativeMethods.ReleaseCapture();
                    NativeMethods.PostMessage(WindowHandle, NativeMethods.WindowsMessage.WM_NCLBUTTONDOWN, (IntPtr)0x2, IntPtr.Zero);
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsInDragRegion(System.Drawing.Point point)
        {
            return _DraggableRegion?.IsVisible(point) == true;
        }

        private void ToogleMaximize()
        {
            Window.WindowState = (Window.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Drawing.Point GetPoint(Message message)
        {
            var lparam = message.LParam;
            var x = NativeMethods.LoWord(lparam.ToInt32());
            var y = NativeMethods.HiWord(lparam.ToInt32());
            return new System.Drawing.Point(x, y);
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Window.Closed -= Window_Closed;
            Window.StateChanged -= Window_StateChanged;
            _ChromeWidgetMessageInterceptor?.ReleaseHandle();
            _ChromeWidgetMessageInterceptor?.DestroyHandle();
            _ChromeWidgetMessageInterceptor = null;
        }
    }
}
