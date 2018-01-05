using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Shell;
using Neutronium.WPF.WPF;

namespace Neutronium.WPF 
{
    public class Chromeless : Behavior<Window>
    {
        private Window Window => AssociatedObject;

        protected override void OnAttached() 
        {
            base.OnAttached();

            ApplyStyle();
            Window.SourceInitialized += (s, e) =>
            {
                var handle = (new WindowInteropHelper(Window)).Handle;
                var sizer = new WindowSizer(Window);
                HwndSource.FromHwnd(handle)?.AddHook(sizer.WindowProc);
            };
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
    }
}
