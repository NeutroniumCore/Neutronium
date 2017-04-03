using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF {
    public class WpfScreen {
        public static WpfScreen Primary => new WpfScreen(System.Windows.Forms.Screen.PrimaryScreen);

        public Rect DeviceBounds => GetRect(this.screen.Bounds);
        public Rect WorkingArea => GetRect(this.screen.WorkingArea);
        public bool IsPrimary => screen.Primary;
        public string DeviceName => screen.DeviceName;

        private readonly Screen screen;

        internal WpfScreen(System.Windows.Forms.Screen screen) {
            this.screen = screen;
        }

        public static IEnumerable<WpfScreen> AllScreens() {
            return System.Windows.Forms.Screen.AllScreens.Select(screen => new WpfScreen(screen));
        }

        public static WpfScreen GetScreenFrom(Window window) {
            var windowInteropHelper = new WindowInteropHelper(window);
            var screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            return new WpfScreen(screen);
        }

        public static WpfScreen GetScreenFrom(System.Windows.Point point) {
            var x = (int) Math.Round(point.X);
            var y = (int) Math.Round(point.Y);

            // are x,y device-independent-pixels ??
            var drawingPoint = new System.Drawing.Point(x, y);
            var screen = System.Windows.Forms.Screen.FromPoint(drawingPoint);
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
