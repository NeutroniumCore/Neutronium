using System.Windows;

namespace Neutronium.WPF.WPF
{
    public static class WindowExtensions
    {
        public static WpfScreen GetCurrentScreen(this Window window)
        {
            return WpfScreen.GetScreenFrom(window);
        }

        public static Rect GetCurrentScreenWorkingArea(this Window window)
        {
            return WpfScreen.GetScreenFrom(window).WorkingArea;
        }
    }
}
