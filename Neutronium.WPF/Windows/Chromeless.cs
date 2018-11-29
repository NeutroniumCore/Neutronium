using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Shell;

namespace Neutronium.WPF 
{
    public class Chromeless : Behavior<Window>
    {
        private Window Window => AssociatedObject;
        public Thickness Thickness { get; set; } = new Thickness(2);

        protected override void OnAttached() 
        {
            base.OnAttached();

            ApplyStyle();
        }

        private void ApplyStyle() 
        {
            var style = new Style(typeof(Window));
            style.Setters.Add(new Setter(Control.BorderThicknessProperty, Thickness));
            style.Setters.Add(new Setter(Window.WindowStyleProperty, WindowStyle.None));
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
    }
}
