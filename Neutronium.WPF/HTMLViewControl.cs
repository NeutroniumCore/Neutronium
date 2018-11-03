using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Neutronium.Core;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using Neutronium.Core.Navigation;
using Neutronium.WPF.Internal;
using System.ComponentModel;

namespace Neutronium.WPF
{
    public class HTMLViewControl : HTMLControlBase
    {
        public static readonly DependencyProperty UriProperty = DependencyProperty.Register("Uri", typeof(Uri),
                                                    typeof(HTMLViewControl), new PropertyMetadata(OnUriChanged));

        private static int _Count = 0;
        public override string UniqueName { get; } = $"HTML ViewControl {_Count++}";

        public Uri Uri
        {
            get => (Uri)GetValue(UriProperty);
            set => SetValue(UriProperty, value);
        }

        public string RelativeSource
        {
            set
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return;

                var path = $"{Assembly.GetExecutingAssembly().GetPath()}\\{value}";
                if (!File.Exists(path))
                    throw ExceptionHelper.Get($"Path not found {path}");

                Uri = new Uri(path);
            }
        }

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(JavascriptBindingMode), typeof(HTMLViewControl), new PropertyMetadata(JavascriptBindingMode.TwoWay));

        public JavascriptBindingMode Mode
        {
            get => (JavascriptBindingMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public HTMLViewControl() : this(new UrlSolver())
        {
        }

        private HTMLViewControl(UrlSolver urlSolver) : base(urlSolver)
        {
            urlSolver.Solver = this;
            DataContextChanged += HTMLViewControl_DataContextChanged;
        }

        private async void HTMLViewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            await CheckNavigation();
        }

        private static async void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var htmlViewControl = (HTMLViewControl)d;
            await htmlViewControl.CheckNavigation();
        }

        private async Task CheckNavigation()
        {
            if ((Uri == null) || (DataContext == null))
                return;

            await NavigateAsyncBase(DataContext, null, Mode);
        }

        private class UrlSolver : IUrlSolver
        {
            public HTMLViewControl Solver { private get; set; }

            Uri IUrlSolver.Solve(object iViewModel, string Id)
            {
                return Solver.Uri;
            }
        }
    }
}
