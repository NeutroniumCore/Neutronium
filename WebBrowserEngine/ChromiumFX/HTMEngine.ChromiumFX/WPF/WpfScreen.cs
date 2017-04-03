using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF {
    public class WpfScreen {
        public static WpfScreen Primary => new WpfScreen(Screen.PrimaryScreen);

        public Rect DeviceBounds => GetRect(_Screen.Bounds);
        public Rect WorkingArea => GetRect(_Screen.WorkingArea);
        public bool IsPrimary => _Screen.Primary;
        public string DeviceName => _Screen.DeviceName;

        private readonly Screen _Screen;

        internal WpfScreen(Screen screen) {
            this._Screen = screen;
        }

        public static IEnumerable<WpfScreen> AllScreens() {
            return Screen.AllScreens.Select(screen => new WpfScreen(screen));
        }

        public static WpfScreen GetScreenFrom(Window window) {
            var windowInteropHelper = new WindowInteropHelper(window);
            var screen = Screen.FromHandle(windowInteropHelper.Handle);
            return new WpfScreen(screen);
        }

        public static WpfScreen GetScreenFrom(System.Windows.Point point) {
            var x = (int) Math.Round(point.X);
            var y = (int) Math.Round(point.Y);

            // are x,y device-independent-pixels ??
            var drawingPoint = new System.Drawing.Point(x, y);
            var screen = Screen.FromPoint(drawingPoint);
            return new WpfScreen(screen);
        }

        private Rect GetRect(Rectangle value) {
            // should x, y, width, height be device-independent-pixels ??
            return new Rect {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }
    }
}
