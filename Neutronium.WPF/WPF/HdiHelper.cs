using System;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Neutronium.WPF
{
    public static class HdiHelper
    {
        [DllImport("user32.dll")]
        private static extern int GetDpiForWindow(IntPtr hWnd);

        private static bool _IsSupported = true;

        public static Matrix GetDisplayScaleFactor(IntPtr windowHandle)
        {
            var matrix = new Matrix(1, 0, 0, 1, 0, 0);
            if (!_IsSupported)
                return matrix;

            try
            {
                var dpi = GetDpiForWindow(windowHandle) / 96f;
                matrix.Scale(dpi, dpi);
            }
            catch
            {
                _IsSupported = false;
            }
            return matrix;
        }
    }
}
