using System.Windows;

namespace Neutronium.WPF
{
    public static class WindowExtensions
    {
        public static WpfScreen GetCurrentScreen(this Window window)
        {
            return WpfScreen.GetScreenFrom(window);
        }
    }
}
