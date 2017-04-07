using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Shell;

namespace Neutronium.WPF 
{
    public class Chromeless : Behavior<Window>
    {
        private Window Window => AssociatedObject;

        protected override void OnAttached() 
        {
            base.OnAttached();

            Window.LocationChanged += ChromelessWindow_LocationChanged;
            ApplyStyle();
            CheckMaxDimension();
        }

        private void ApplyStyle() 
        {
            var style = new Style(typeof(Window));
            style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1)));
            var windowChrome = new WindowChrome
            {
                ResizeBorderThickness = new Thickness(6),
                CaptionHeight = 0,
                CornerRadius = new CornerRadius(2,2,1,1),
                GlassFrameThickness = new Thickness(0)
            };
            style.Setters.Add(new Setter(WindowChrome.WindowChromeProperty, windowChrome));
            Window.Style = style;
        }

        private void ChromelessWindow_LocationChanged(object sender, EventArgs e) 
        {
            CheckMaxDimension();
        }

        private void CheckMaxDimension()
        {
            var area = Window.GetCurrentScreenWorkingArea();
            Window.MaxHeight = area.Height;
            Window.MaxWidth = area.Width;
        }

        protected override void OnDetaching() 
        {
            base.OnDetaching();
            Window.LocationChanged -= ChromelessWindow_LocationChanged;
        }
    }
}
