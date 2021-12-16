using Chromium.Event;
using Chromium.WebBrowser;
using Neutronium.WebBrowserEngine.ChromiumFx.Util;
using Neutronium.WPF;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Neutronium.Core.Exceptions;
using System.Windows.Forms.Integration;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF {
    public partial class ChromiumFxControl
    {
        private Window _Window;
        private IntPtr _WindowHandle;
        private IntPtr _ChromeWidgetHostHandle;
        private BrowserWidgetMessageInterceptor _ChromeWidgetMessageInterceptor;
        private Region _DraggableRegion = null;
        private IntPtr _BrowserHandle;
        private Matrix _Matrix = new Matrix(1, 0, 0, 1, 0, 0);

        public ChromiumWebBrowser ChromiumWebBrowser { get; }

        public ChromiumFxControl(bool useNeutroniumSettings)
        {
            InitializeComponent();
            this.Loaded += ChromiumFxControl_Loaded;
            ChromiumWebBrowser = new ChromiumWebBrowser(false)
            {
                Dock = DockStyle.Fill
            };
            var settings = useNeutroniumSettings ?
                 NeutroniumSettings.NeutroniumBrowserSettings : ChromiumWebBrowser.DefaultBrowserSettings;
            ChromiumWebBrowser.CreateBrowser(settings);
            Host.Child = ChromiumWebBrowser;

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
            _ChromeWidgetMessageInterceptor = new BrowserWidgetMessageInterceptor(this.ChromiumWebBrowser, _ChromeWidgetHostHandle, OnWebBrowserMessage);
        }

        private void ChromiumFxControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ChromiumFxControl_Loaded;
            _Window = Window.GetWindow(this);
            if (_Window == null)
                throw ExceptionHelper.Get("Neutronium UserControls should be inserted in Window before being loaded");
            
            _WindowHandle = new WindowInteropHelper(_Window).Handle;
            _Window.Closed += Window_Closed;
            _Window.LocationChanged += _Window_LocationChanged;
            _Window.StateChanged += Window_StateChanged;
            UpdateDpiMatrix();
        }

        private async void Window_StateChanged(object sender, EventArgs e)
        {
            //Do not remove before intensive tests.
            //When resizing windows, chromium component may not redraw correctly
            //This fix makes sure that after maximizing the window is correctly redrawn.
            if (_Window.WindowState == WindowState.Minimized)
                return;

            await Task.Delay(10);
            ChromiumWebBrowser.Refresh();
        }

        private void _Window_LocationChanged(object sender, EventArgs e)
        {
            UpdateDpiMatrix();
        }

        private void UpdateDpiMatrix()
        {
            _Matrix = HdiHelper.GetDisplayScaleFactor(_WindowHandle);
        }

        private bool OnWebBrowserMessage(Message message)
        {
            switch (message.Msg)
            {
                case NativeMethods.WindowsMessage.WM_LBUTTONDBLCLK:
                    if (!IsInDragRegion(message))
                        break;

                    Dispatcher.BeginInvoke(new Action(ToggleMaximize));
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
            if (_DraggableRegion == null)
                return false;

            var point = GetPoint(message);
            return _DraggableRegion.IsVisible(point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Drawing.Point GetPoint(Message message)
        {
            var lparam = message.LParam;
            var x = NativeMethods.LoWord(lparam.ToInt32());
            var y = NativeMethods.HiWord(lparam.ToInt32());
            return new System.Drawing.Point(x, y);
        }

        private void ToggleMaximize()
        {
            _Window.WindowState = (_Window.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            _Window.Closed -= Window_Closed;
            _Window.LocationChanged -= _Window_LocationChanged;
            _Window.StateChanged -= Window_StateChanged;
            _ChromeWidgetMessageInterceptor?.ReleaseHandle();
            _ChromeWidgetMessageInterceptor?.DestroyHandle();
            _ChromeWidgetMessageInterceptor = null;
        }
    }
}
