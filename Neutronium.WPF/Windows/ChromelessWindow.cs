using System.Windows;

namespace Neutronium.WPF.Windows
{
    public class ChromelessWindow : Window
    {
        public ChromelessWindow()
        {
            LocationChanged += ChromelessWindow_LocationChanged;
        }

        private void ChromelessWindow_LocationChanged(object sender, System.EventArgs e)
        {
            var area = this.GetCurrentScreenWorkingArea();
            MaxHeight = area.Height;
            MaxWidth = area.Width;
        }
    }
}
