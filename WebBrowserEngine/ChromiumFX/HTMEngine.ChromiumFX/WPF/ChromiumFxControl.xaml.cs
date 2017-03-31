using Chromium.WebBrowser;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public partial class ChromiumFxControl
    {
        internal ChromiumWebBrowser WebBrowser => ChromiumWebBrowser;
        private Window Window { get; set; }
        private IntPtr FormHandle => _BrowserHandle;
        private const int BORDER_WIDTH = 8;
        public bool Borderless { get; set; } = true;
        public bool Resizable { get; set; } = true;
        

        private BrowserWidgetMessageInterceptor _ChromeWidgetMessageInterceptor;
        private Region draggableRegion = null;

        public ChromiumFxControl() 
        {
            InitializeComponent();
            this.Loaded += ChromiumFxControl_Loaded;
            ChromiumWebBrowser.BrowserCreated += ChromiumWebBrowser_BrowserCreated;
            var dragHandler =ChromiumWebBrowser.DragHandler;
            dragHandler.OnDragEnter += (o, e) => { e.SetReturnValue(true); };
            dragHandler.OnDraggableRegionsChanged += DragHandler_OnDraggableRegionsChanged;
        }

        private void DragHandler_OnDraggableRegionsChanged(object sender, Chromium.Event.CfxOnDraggableRegionsChangedEventArgs args)
        {
            args.Regions.Aggregate(draggableRegion, (current, region) => 
            {
                var rect = new Rectangle(region.Bounds.X, region.Bounds.Y, region.Bounds.Width, region.Bounds.Height);
                if (current == null)
                    return new Region(rect);

                if (region.Draggable) {
                    current.Union(rect);
                }
                else {
                    current.Exclude(rect);
                }
                return current;
            });
        }

        //if (regions.Length > 0) {
            //    foreach (var region in regions) {
            //        var rect = new Rectangle(region.Bounds.X, region.Bounds.Y, region.Bounds.Width, region.Bounds.Height);

            //        if (draggableRegion == null) {
            //            draggableRegion = new Region(rect);
            //        }
            //        else {
            //            if (region.Draggable) {
            //                draggableRegion.Union(rect);
            //            }
            //            else {
            //                draggableRegion.Exclude(rect);
            //            }
            //        }
            //    }
            //}
        //}
        private IntPtr _BrowserHandle;
        private async void ChromiumWebBrowser_BrowserCreated(object sender, Chromium.WebBrowser.Event.BrowserCreatedEventArgs e)
        {
            _BrowserHandle = e.Browser.Host.WindowHandle;
            await Task.Delay(1000);

            IntPtr chromeWidgetHostHandle;
            if (ChromeWidgetHandleFinder.TryFindHandle(_BrowserHandle, out chromeWidgetHostHandle)) 
                _ChromeWidgetMessageInterceptor = new BrowserWidgetMessageInterceptor(this.ChromiumWebBrowser, chromeWidgetHostHandle, OnWebBroswerMessage);         
        }

        private void ChromiumFxControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ChromiumFxControl_Loaded;
            Window = Window.GetWindow(this);
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
            if (message.Msg == NativeMethods.WindowsMessage.WM_MOUSEACTIVATE) {
                var topLevelWindowHandle = message.WParam;
                NativeMethods.PostMessage(topLevelWindowHandle, NativeMethods.WindowsMessage.WM_NCLBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            }

            //var formHandle = IntPtr.Zero;
            //Action doact = () => formHandle = FormHandle;
            //Dispatcher.Invoke(doact);

            if (message.Msg == NativeMethods.WindowsMessage.WM_LBUTTONDOWN) {

                var x = NativeMethods.LoWord(message.LParam.ToInt32());
                var y = NativeMethods.HiWord(message.LParam.ToInt32());

                var dragable = (draggableRegion != null && draggableRegion.IsVisible(new System.Drawing.Point(x, y)));

                //var dir = GetDirection(x, y);
                var dir = 0;
                Action act = () => dir = GetDirection(x, y);
                Dispatcher.Invoke(act);

                if (dir != NativeMethods.HitTest.HTCLIENT && Borderless) 
                {
                    NativeMethods.PostMessage(FormHandle, NativeMethods.DefMessages.WM_CEF_RESIZE_CLIENT, (IntPtr) dir, message.LParam);
                    return true;
                }
                else if (dragable) {
                    NativeMethods.PostMessage(FormHandle, NativeMethods.DefMessages.WM_CEF_DRAG_APP, message.WParam, message.LParam);
                    return true;
                }
            }

            if (message.Msg == NativeMethods.WindowsMessage.WM_LBUTTONDBLCLK && Resizable) {
                var x = NativeMethods.LoWord(message.LParam.ToInt32());
                var y = NativeMethods.HiWord(message.LParam.ToInt32());

                var dragable = (draggableRegion != null && draggableRegion.IsVisible(new System.Drawing.Point(x, y)));

                if (dragable) {
                    NativeMethods.PostMessage(FormHandle, NativeMethods.DefMessages.WM_CEF_TITLEBAR_LBUTTONDBCLICK, message.WParam, message.LParam);

                    return true;
                }

            }

            if (message.Msg == NativeMethods.WindowsMessage.WM_MOUSEMOVE) {
                var x = NativeMethods.LoWord(message.LParam.ToInt32());
                var y = NativeMethods.HiWord(message.LParam.ToInt32());

                if (Resizable && Borderless) 
                {
                    var direction = 0;
                    Action act = () => direction = GetDirection(x, y);
                    Dispatcher.Invoke(act);

                    if (direction != NativeMethods.HitTest.HTCLIENT) {
                        NativeMethods.PostMessage(FormHandle, NativeMethods.DefMessages.WM_CEF_EDGE_MOVE, (IntPtr) direction, message.LParam);

                        return true;
                    }
                }

                NativeMethods.SendMessage(FormHandle, NativeMethods.WindowsMessage.WM_MOUSEMOVE, message.WParam, message.LParam);
            }
            return false;
        }

        private int GetDirection(int x, int y) 
        {
            var dir = NativeMethods.HitTest.HTCLIENT;

            var window = Window.GetWindow(this);

            if (window?.WindowState != WindowState.Normal) {
                return dir;
            }

            if (x < BORDER_WIDTH & y < BORDER_WIDTH) {
                dir = NativeMethods.HitTest.HTTOPLEFT;
            }
            else if (x < BORDER_WIDTH & y > this.Height - BORDER_WIDTH) {
                dir = NativeMethods.HitTest.HTBOTTOMLEFT;

            }
            else if (x > this.Width - BORDER_WIDTH & y > this.Height - BORDER_WIDTH) {
                dir = NativeMethods.HitTest.HTBOTTOMRIGHT;

            }
            else if (x > this.Width - BORDER_WIDTH & y < BORDER_WIDTH) {
                dir = NativeMethods.HitTest.HTTOPRIGHT;

            }
            else if (x < BORDER_WIDTH) {
                dir = NativeMethods.HitTest.HTLEFT;

            }
            else if (x > this.Width - BORDER_WIDTH) {
                dir = NativeMethods.HitTest.HTRIGHT;

            }
            else if (y < BORDER_WIDTH) {
                dir = NativeMethods.HitTest.HTTOP;

            }
            else if (y > this.Height - BORDER_WIDTH) {
                dir = NativeMethods.HitTest.HTBOTTOM;
            }
            return dir;
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
