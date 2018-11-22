using System.Windows;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Neutronium.WebBrowserEngine.ChromiumFx.Util;
using Chromium.Event;
using Neutronium.WPF;
using System.Windows.Interop;
using System.Drawing.Drawing2D;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public partial class ChromiumFxControl
    {
        private Window _Window;
        private IntPtr _WindowHandle;
        private IntPtr _ChromeWidgetHostHandle;
        private BrowserWidgetMessageInterceptor _ChromeWidgetMessageInterceptor;
        private Region _DraggableRegion = null;
        private IntPtr _BrowserHandle;
        private Matrix _Matrix;

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
            _DraggableRegion = args.Regions.Aggregate(new Region(), (current, region) =>
            {
                var rect = new Rectangle(region.Bounds.X, region.Bounds.Y, region.Bounds.Width, region.Bounds.Height);

                if (region.Draggable)
                    current.Union(rect);
                else
                    current.Exclude(rect);

                return current;
            });

            _DraggableRegion.Transform(_Matrix);
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
            _Window = Window.GetWindow(this);
            _WindowHandle = new WindowInteropHelper(_Window).Handle;
            _Window.Closed += Window_Closed;
            _Window.LocationChanged += _Window_LocationChanged;
            UpdateDpiMatrix();
        }

        private void _Window_LocationChanged(object sender, EventArgs e)
        {
            UpdateDpiMatrix();
        }

        private void UpdateDpiMatrix()
        {
            _Matrix = HdiHelper.GetDisplayScaleFactor(_WindowHandle);
        }

        private bool OnWebBroswerMessage(Message message)
        {
            switch (message.Msg)
            {
                case NativeMethods.WindowsMessage.WM_LBUTTONDBLCLK:
                    if (!IsInDragRegion(message))
                        break;

                    Dispatcher.Invoke(ToogleMaximize);
                    return true;

                case NativeMethods.WindowsMessage.WM_LBUTTONDOWN:
                    if (!IsInDragRegion(message))
                        return false;

                    NativeMethods.ReleaseCapture();
                    NativeMethods.PostMessage(_WindowHandle, NativeMethods.WindowsMessage.WM_NCLBUTTONDOWN, (IntPtr)NativeMethods.HitTest.HTCAPTION, IntPtr.Zero);
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsInDragRegion(Message message) 
        {
            var point = GetPoint(message);
            return _DraggableRegion?.IsVisible(point) == true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Drawing.Point GetPoint(Message message)
        {
            var lparam = message.LParam;
            var x = NativeMethods.LoWord(lparam.ToInt32());
            var y = NativeMethods.HiWord(lparam.ToInt32());
            return new System.Drawing.Point(x, y);
        }

        private void ToogleMaximize()
        {
            _Window.WindowState = (_Window.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            _Window.Closed -= Window_Closed;
            _Window.LocationChanged -= _Window_LocationChanged;
            _ChromeWidgetMessageInterceptor?.ReleaseHandle();
            _ChromeWidgetMessageInterceptor?.DestroyHandle();
            _ChromeWidgetMessageInterceptor = null;
        }
    }
}
