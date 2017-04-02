using Chromium.WebBrowser;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Neutronium.WebBrowserEngine.ChromiumFx.Util;
using Gma.System.MouseKeyHook;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public partial class ChromiumFxControl
    {
        internal ChromiumWebBrowser WebBrowser => ChromiumWebBrowser;
        private Window Window { get; set; }
        private IntPtr WindowHandle { get; set; }
        private IntPtr FormHandle => _BrowserHandle;
        private IntPtr _ChromeWidgetHostHandle;
        private System.Windows.Point _DragOffset = new System.Windows.Point();
        private bool _Dragging = false;
        private BrowserWidgetMessageInterceptor _ChromeWidgetMessageInterceptor;
        private Region _DraggableRegion = null;
        private Rectangle _Rectange = new Rectangle(0,0,0,0);
        private bool _Listenning;
        private IDisposableMouseEvents _listener;

        public ChromiumFxControl()
        {
            InitializeComponent();
            this.Loaded += ChromiumFxControl_Loaded;
            ChromiumWebBrowser.BrowserCreated += ChromiumWebBrowser_BrowserCreated;
            var dragHandler = ChromiumWebBrowser.DragHandler;
            dragHandler.OnDragEnter += (o, e) => { e.SetReturnValue(true); };
            dragHandler.OnDraggableRegionsChanged += DragHandler_OnDraggableRegionsChanged;
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

        private IntPtr _BrowserHandle;
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
                case NativeMethods.WindowsMessage.WM_MOUSEACTIVATE:
                    var topLevelWindowHandle = message.WParam;
                    NativeMethods.PostMessage(topLevelWindowHandle, NativeMethods.WindowsMessage.WM_NCLBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                    break;

                case NativeMethods.WindowsMessage.WM_LBUTTONDBLCLK:
                    _Dragging = false;
                    if (!IsInDragRegion(GetPoint(message.LParam)))
                        break;

                    Dispatcher.Invoke(() => ToogleMaximize());
                    return true;

                case NativeMethods.WindowsMessage.WM_LBUTTONDOWN:
                    var point = GetPoint(message.LParam);
                    _Dragging = IsInDragRegion(point);

                    if (!_Dragging)
                        break;

                    NativeMethods.PostMessage(FormHandle, NativeMethods.WindowsMessage.WM_LBUTTONDOWN, message.WParam, message.LParam);
                    Dispatcher.Invoke(() => DragInit(point));
                    return true;

                case NativeMethods.WindowsMessage.WM_LBUTTONUP:
                    UnListen();
                    _Dragging = false;
                    break;

                case NativeMethods.WindowsMessage.WM_MOUSEMOVE:
                    _Dragging = _Dragging && (int)message.WParam == NativeMethods.windowsParam.MK_LBUTTON;

                    if (_Dragging)
                        Dispatcher.Invoke(() => OnMouseDown(GetPoint(message.LParam)));

                    NativeMethods.SendMessage(FormHandle, NativeMethods.WindowsMessage.WM_MOUSEMOVE, message.WParam, message.LParam);
                    break;

                case NativeMethods.WindowsMessage.WM_MOUSELEAVE:
                    if (_Dragging)
                        Listen();
                    break;
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

        private void Listen()
        {
            if (_Listenning)
                return;

            _Listenning = true;
            _listener = _listener ?? Hook.GlobalEvents();
            _listener.MouseMove += Hook_MouseMove;
        }

        private void UnListen()
        {
            if (!_Listenning)
                return;

            _listener.MouseMove -= Hook_MouseMove;
            _Listenning = false;
            return;
        }

        private void Hook_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_Dragging)
            {
                UnListen();
                return;
            }

            Dispatcher.Invoke(() =>
            {
                Window.Left = _DragOffset.X + e.X;
                Window.Top = _DragOffset.Y + e.Y;
            });
        }

        private System.Windows.Point GetPoint(IntPtr lparam)
        {
            var x = NativeMethods.LoWord(lparam.ToInt32());
            var y = NativeMethods.HiWord(lparam.ToInt32());
            return new System.Windows.Point(x, y);
        }

        private System.Drawing.Point Convert(System.Windows.Point point)
        {
            return new System.Drawing.Point((int)point.X, (int)point.Y);
        }

        private System.Windows.Point RealPixelsToWpf(System.Windows.Point point)
        {
            return Window.PointToScreen(point);
        }

        private System.Windows.Point RealPixelsToWpf(double x, double y)
        {
            return Window.PointToScreen(new System.Windows.Point(x, y));
        }

        private void DragInit(System.Windows.Point point)
        {
            _Dragging = true;
            ComputeDragOffset(RealPixelsToWpf(point));
        }

        protected void OnMouseDown(System.Windows.Point point)
        {
            if (!_Dragging)
                return;

            var off = RealPixelsToWpf(point);

            if (MinimizeIfNeeded(off))
                return;
            
            Window.Left = _DragOffset.X + off.X;
            Window.Top = _DragOffset.Y + off.Y;
        }

        private bool MinimizeIfNeeded(System.Windows.Point point)
        {
            if (Window.WindowState != WindowState.Maximized)
                return false;
    
            Window.Top = 0;
            Window.Left = GetNormalX(point.X);
            Window.WindowState = WindowState.Normal;

            ComputeDragOffset(point);
            return true;
        }

        private void ComputeDragOffset(System.Windows.Point point)
        {
            _DragOffset = new System.Windows.Point(Window.Left - point.X, Window.Top - point.Y);
        }

        private double GetNormalX(double originalx)
        {
            var screenWith = SystemParameters.WorkArea.Width;
            var currentWidth = RealPixelsToWpf(_Rectange.Width, 0).X;
            var futureWidth = currentWidth * Window.RestoreBounds.Width / screenWith;
            var x = originalx - futureWidth / 2;
            if (x < 0)
                return 0;
            
            return (x + futureWidth > screenWith) ? screenWith - futureWidth : x;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Window.Closed -= Window_Closed;
            Window.StateChanged -= Window_StateChanged;

            _ChromeWidgetMessageInterceptor?.ReleaseHandle();
            _ChromeWidgetMessageInterceptor?.DestroyHandle();
            _ChromeWidgetMessageInterceptor = null;

            _listener?.Dispose();
            _listener = null;
        }
    }
}
