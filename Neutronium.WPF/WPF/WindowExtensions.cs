using System.Windows;

namespace Neutronium.WPF
{
    public static class WindowExtensions
    {
        public static WpfScreen GetCurrentScreen(this Window window)
        {
            return WpfScreen.GetScreenFrom(window);
        }

        public static Rect GetCurrentScreenWorkingArea(this Window window)
        {
            var screen = WpfScreen.GetScreenFrom(window);
            return screen.DeviceBounds; //instead of WorkingArea which on some test less than expected
        }
    }
}
