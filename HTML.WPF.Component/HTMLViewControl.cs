using System;
using System.IO;
using System.Reflection;
using System.Windows;

using MVVM.HTML.Core;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Navigation;
using MVVM.HTML.Core.Infra;

namespace HTML_WPF.Component
{
    public class HTMLViewControl : HTMLControlBase, IWebViewLifeCycleManager, IDisposable
    {
        public static readonly DependencyProperty UriProperty = DependencyProperty.Register("Uri", typeof(Uri), typeof(HTMLViewControl));

        public Uri Uri
        {
            get { return (Uri)GetValue(UriProperty); }
            internal set { SetValue(UriProperty, value); }
        }

        public string RelativeSource
        {
            set
            {
                var path = string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), value);
                if (!File.Exists(path))
                    throw ExceptionHelper.Get(string.Format("Path not found {0}", path));
                Uri = new Uri(path);

                if (DataContext != null)
                    NavigateAsyncBase(DataContext, null, Mode).DoNotWait();
            }
        }

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(JavascriptBindingMode), typeof(HTMLViewControl), new PropertyMetadata(JavascriptBindingMode.TwoWay));

        public JavascriptBindingMode Mode
        {
            get { return (JavascriptBindingMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        private UrlSolver _UrlSolver;

        public HTMLViewControl() : this(new UrlSolver())
        {
        }

        private HTMLViewControl(UrlSolver iIUrlSolver): base(iIUrlSolver)
        {
            _UrlSolver = iIUrlSolver;
            _UrlSolver.Solver = this;
            DataContextChanged += HTMLViewControl_DataContextChanged;
        }

        private async void HTMLViewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Uri != null)
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
