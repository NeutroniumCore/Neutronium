using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;

namespace Neutronium.WPF
{
    public class Chromeless : Behavior<Window>
    {
        private Window Window => AssociatedObject;

        public Thickness Thickness { get; set; } = new Thickness(2);
        public Thickness MaximizedThickness { get; set; } = new Thickness(8);
        public Thickness ResizeBorderThickness { get; set; } = new Thickness(6);
        public Brush BackGround { get; set; } = Brushes.Black;

        protected override void OnAttached()
        {
            base.OnAttached();

            ApplyStyle();
        }

        private void ApplyStyle()
        {
            var style = Window.Style ?? new Style(typeof(Window));
            var setters = style.Setters;
            setters.Add(new Setter(Control.BorderThicknessProperty, Thickness));
            setters.Add(new Setter(Control.BorderBrushProperty, BackGround));
            setters.Add(new Setter(Control.BackgroundProperty, BackGround));
            var windowChrome = new WindowChrome
            {
                ResizeBorderThickness = ResizeBorderThickness,
                CaptionHeight = 0,
                CornerRadius = new CornerRadius(2, 2, 1, 1),
                GlassFrameThickness = new Thickness(0)
            };
            setters.Add(new Setter(WindowChrome.WindowChromeProperty, windowChrome));
            var trigger = new Trigger
            {
                Property = Window.WindowStateProperty,
                Value = WindowState.Maximized
            };
            trigger.Setters.Add(new Setter(Control.BorderThicknessProperty, MaximizedThickness));
            style.Triggers.Add(trigger);
            Window.Style = style;
        }
    }
}
